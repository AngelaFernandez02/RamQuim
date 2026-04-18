using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

[Route("api/[controller]")]
[ApiController]
public class InventarioController : ControllerBase
{
    private readonly RamQuimContext _context;

    public InventarioController(RamQuimContext context)
    {
        _context = context;
    }

    // =========================
    // CONSULTAR INVENTARIO
    // =========================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventarioDTO>>> GetInventario()
    {
        var inventario = await _context.Inventarios
            .Join(_context.Productos.Where(p => p.Estatus == true),
                inv => inv.IdProducto,
                prod => prod.IdProducto,
                (inv, prod) => new InventarioDTO
                {
                    IdInventario = inv.IdInventario,
                    IdProducto = prod.IdProducto,
                    nombreProducto = prod.NombreProducto,
                    descripcion = prod.Descripcion,
                    presentacion = prod.Presentacion,
                    imagen = prod.Imagen,
                    CantidadDisponible = inv.CantidadDisponible
                })
            .ToListAsync();

        return Ok(inventario);
    }

    // =========================
    // ENTRADA DE PRODUCTO
    // =========================
    [HttpPost("entrada")]
    public async Task<IActionResult> EntradaProducto([FromBody] MovimientoInventarioDTO mov)
    {
        if (mov.Cantidad <= 0)
        {
            return BadRequest("La cantidad debe ser mayor a 0");
        }

        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.IdProducto == mov.IdProducto);

        // Si no existe inventario se crea
        if (inventario == null)
        {
            inventario = new Inventario
            {
                IdProducto = mov.IdProducto,
                CantidadDisponible = mov.Cantidad
            };

            _context.Inventarios.Add(inventario);
        }
        else
        {
            inventario.CantidadDisponible += mov.Cantidad;
        }

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Entrada registrada correctamente" });
    }

    // =========================
    // SALIDA DE PRODUCTO
    // =========================
    [HttpPost("salida")]
    public async Task<IActionResult> SalidaProducto([FromBody] MovimientoInventarioDTO mov)
    {
        if (mov.Cantidad <= 0)
        {
            return BadRequest("La cantidad debe ser mayor a 0");
        }

        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.IdProducto == mov.IdProducto);

        if (inventario == null)
        {
            return NotFound("El producto no tiene inventario");
        }

        if (inventario.CantidadDisponible < mov.Cantidad)
        {
            return BadRequest("No hay suficiente inventario");
        }

        inventario.CantidadDisponible -= mov.Cantidad;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Salida registrada correctamente" });
    }

    // =========================
    // CONSULTAR INVENTARIO POR PRODUCTO
    // =========================
    [HttpGet("producto/{idProducto}")]
    public async Task<ActionResult<InventarioDTO>> GetInventarioProducto(int idProducto)
    {
        var inventario = await _context.Inventarios
            .Join(_context.Productos.Where(p => p.Estatus == true),
                inv => inv.IdProducto,
                prod => prod.IdProducto,
                (inv, prod) => new InventarioDTO
                {
                    IdInventario = inv.IdInventario,
                    IdProducto = prod.IdProducto,
                    nombreProducto = prod.NombreProducto,
                    descripcion = prod.Descripcion,
                    presentacion = prod.Presentacion,
                    imagen = prod.Imagen,
                    CantidadDisponible = inv.CantidadDisponible
                })
            .FirstOrDefaultAsync(i => i.IdProducto == idProducto);

        if (inventario == null)
        {
            return NotFound("Producto no encontrado en inventario o está desactivado");
        }

        return Ok(inventario);
    }
}