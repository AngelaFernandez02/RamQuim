using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

[Route("api/[controller]")]
[ApiController]
public class ProductoController : ControllerBase
{
    private readonly RamQuimContext _context;

    public ProductoController(RamQuimContext context)
    {
        _context = context;
    }

   
    // OBTENER TODOS LOS PRODUCTOS ACTIVOS

    [HttpGet]
    public async Task<IActionResult> GetProductos()
    {
        var listaProductos = await _context.Productos
            .Where(p => p.Estatus == true)
            .Select(p => new
            {
                p.IdProducto,
                p.NombreProducto,
                p.Descripcion,
                p.Presentacion,
                p.PrecioUnitario,
                p.Imagen
            })
            .ToListAsync();

        return Ok(listaProductos);
    }

  
    // OBTENER PRODUCTO POR ID
 
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProducto(int id)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.IdProducto == id && p.Estatus == true);

        if (producto == null)
            return NotFound();

        return Ok(producto);
    }

   

   
    // CREAR PRODUCTO Y REGISTRAR INVENTARIO AUTOMÁTICO
  
    [HttpPost]
    public async Task<IActionResult> CrearProducto([FromForm] ProductoCreateDTO dto)
    {
        string? nombreArchivo = null;

        // Guardar imagen si existe
        if (dto.Imagen != null)
        {
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/productos");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Imagen.FileName);
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await dto.Imagen.CopyToAsync(stream);
            }
        }

        // Crear producto
        var producto = new Producto
        {
            NombreProducto = dto.NombreProducto,
            Descripcion = dto.Descripcion,
            Presentacion = dto.Presentacion,
            PrecioUnitario = dto.PrecioUnitario,
            Imagen = nombreArchivo,
            Estatus = true
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(); // Guardamos para obtener el IdProducto generado

        // Crear inventario con cantidad 0 automáticamente
        var inventario = new Inventario
        {
            IdProducto = producto.IdProducto,
            CantidadDisponible = 0
        };

        _context.Inventarios.Add(inventario);
        await _context.SaveChangesAsync();

        // Devolver solo los datos necesarios
        return Ok(new
        {
            producto.IdProducto,
            producto.NombreProducto,
            producto.Descripcion,
            producto.Presentacion,
            producto.PrecioUnitario,
            producto.Imagen,
            CantidadInventario = inventario.CantidadDisponible
        });
    }

    
    // DTO ACTUALIZAR PRODUCTO
   
    public class ProductoUpdateDTO
    {
        public string? NombreProducto { get; set; }
        public string? Descripcion { get; set; }
        public string? Presentacion { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public IFormFile? Imagen { get; set; }
        public bool? Estatus { get; set; }
    }

    
    // ACTUALIZAR PRODUCTO
    
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarProducto(int id, [FromForm] ProductoUpdateDTO dto)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return NotFound();

        if (!string.IsNullOrEmpty(dto.NombreProducto))
            producto.NombreProducto = dto.NombreProducto;

        if (!string.IsNullOrEmpty(dto.Descripcion))
            producto.Descripcion = dto.Descripcion;

        if (!string.IsNullOrEmpty(dto.Presentacion))
            producto.Presentacion = dto.Presentacion;

        if (dto.PrecioUnitario.HasValue)
            producto.PrecioUnitario = dto.PrecioUnitario.Value;

        if (dto.Estatus.HasValue)
            producto.Estatus = dto.Estatus.Value;

        // NUEVA IMAGEN
        if (dto.Imagen != null)
        {
            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/productos");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Imagen.FileName);
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await dto.Imagen.CopyToAsync(stream);
            }

            producto.Imagen = nombreArchivo;
        }

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Producto actualizado correctamente" });
    }

    
    // ELIMINAR PRODUCTO (BORRADO LOGICO)
  
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
            return NotFound();

        producto.Estatus = false;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Producto eliminado correctamente" });
    }
}