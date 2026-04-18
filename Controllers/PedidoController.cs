using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly RamQuimContext _context;

    public PedidosController(RamQuimContext context)
    {
        _context = context;
    }

   
    // DTOs

    public class PedidoDTO
    {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<DetallePedidoDTO> Detalles { get; set; } = new List<DetallePedidoDTO>();
    }

    public class DetallePedidoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class PedidoCreateDTO
    {
        public int IdCliente { get; set; }
        public List<DetallePedidoCreateDTO> Detalles { get; set; } = new List<DetallePedidoCreateDTO>();
    }

    public class DetallePedidoCreateDTO
    {
        public int IdProducto { get; set; }
        public decimal Cantidad { get; set; }
    }

    
    // Crear pedido
   
    [HttpPost("Crear")]
    public async Task<IActionResult> CrearPedido([FromBody] PedidoCreateDTO dto)
    {
        if (dto.Detalles.Count == 0)
            return BadRequest("Debe agregar al menos un producto al pedido.");

        var pedido = new Pedido
        {
            IdCliente = dto.IdCliente,
            FechaPedido = DateTime.Now,
            Estado = "Pendiente"
        };

        decimal totalPedido = 0m;

        foreach (var detalleDto in dto.Detalles)
        {
            // Obtenemos el producto desde la base de datos
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == detalleDto.IdProducto);

            if (producto == null)
                return BadRequest($"Producto con ID {detalleDto.IdProducto} no existe.");

            // Validamos inventario disponible
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.IdProducto == detalleDto.IdProducto);

            if (inventario == null)
                return BadRequest($"No hay inventario registrado para el producto {producto.NombreProducto}.");

            if (inventario.CantidadDisponible < detalleDto.Cantidad)
                return BadRequest($"Inventario insuficiente para el producto {producto.NombreProducto}.");

            decimal precioUnitario = producto.PrecioUnitario;
            decimal subtotal = precioUnitario * detalleDto.Cantidad;
            totalPedido += subtotal;

            pedido.DetallePedidos.Add(new DetallePedido
            {
                IdProducto = detalleDto.IdProducto,
                Cantidad = detalleDto.Cantidad,
                PrecioUnitario = precioUnitario,
                Subtotal = subtotal
            });
        }

        pedido.Total = totalPedido;

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // Map a DTO para devolver
        var pedidoDTO = await _context.Pedidos
            .Where(p => p.IdPedido == pedido.IdPedido)
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdCliente = p.IdCliente,
                FechaPedido = p.FechaPedido,
                Total = (decimal)p.Total,
                Estado = p.Estado,
                Detalles = p.DetallePedidos.Select(d => new DetallePedidoDTO
                {
                    IdProducto = d.IdProducto,
                    NombreProducto = d.IdProductoNavigation.NombreProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).FirstOrDefaultAsync();

        return Ok(pedidoDTO);
    }

  
    // Pedidos pendientes

    [HttpGet("Pendientes")]
    public async Task<IActionResult> PedidosPendientes()
    {
        var pedidos = await _context.Pedidos
            .Where(p => p.Estado == "Pendiente")
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdCliente = p.IdCliente,
                FechaPedido = p.FechaPedido,
                Total = (decimal)p.Total,
                Estado = p.Estado,
                Detalles = p.DetallePedidos.Select(d => new DetallePedidoDTO
                {
                    IdProducto = d.IdProducto,
                    NombreProducto = d.IdProductoNavigation.NombreProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToListAsync();

        return Ok(pedidos);
    }

    // Confirmar pedido
   
    [HttpPost("Confirmar/{id}")]
    public async Task<IActionResult> ConfirmarPedido(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.DetallePedidos)
            .ThenInclude(d => d.IdProductoNavigation)
            .FirstOrDefaultAsync(p => p.IdPedido == id);

        if (pedido == null) return NotFound("Pedido no encontrado");
        if (pedido.Estado != "Pendiente") return BadRequest("Pedido ya confirmado");

        foreach (var detalle in pedido.DetallePedidos)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.IdProducto == detalle.IdProducto);

            if (inventario == null)
                return BadRequest($"No hay inventario registrado para el producto {detalle.IdProductoNavigation.NombreProducto}");

            if (inventario.CantidadDisponible < detalle.Cantidad)
                return BadRequest($"Inventario insuficiente para el producto {detalle.IdProductoNavigation.NombreProducto}");

            inventario.CantidadDisponible -= detalle.Cantidad;
        }

        pedido.Estado = "Listo";
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Pedido confirmado y descontado del inventario" });
    }

    
    // Pedidos por cliente
  
    [HttpGet("Cliente/{idCliente}")]
    public async Task<IActionResult> PedidosPorCliente(int idCliente)
    {
        var pedidos = await _context.Pedidos
            .Where(p => p.IdCliente == idCliente)
            .Select(p => new PedidoDTO
            {
                IdPedido = p.IdPedido,
                IdCliente = p.IdCliente,
                FechaPedido = p.FechaPedido,
                Total = (decimal)p.Total,
                Estado = p.Estado,
                Detalles = p.DetallePedidos.Select(d => new DetallePedidoDTO
                {
                    IdProducto = d.IdProducto,
                    NombreProducto = d.IdProductoNavigation.NombreProducto,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToListAsync();

        return Ok(pedidos);
    }
}