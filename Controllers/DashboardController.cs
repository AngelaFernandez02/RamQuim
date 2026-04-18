using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

namespace RamQuim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly RamQuimContext _context;

        public DashboardController(RamQuimContext context)
        {
            _context = context;
        }

        // 1️⃣ Total de productos ACTIVOS
        [HttpGet("total-productos")]
        public async Task<IActionResult> TotalProductos()
        {
            var total = await _context.Productos
                .Where(p => p.Estatus == true)
                .CountAsync();

            return Ok(new { totalProductos = total });
        }

        // 2️⃣ Total de pedidos (solo los listos o completados)
        [HttpGet("total-pedidos")]
        public async Task<IActionResult> TotalPedidos()
        {
            var total = await _context.Pedidos
                .Where(p => p.Estado == "Listo")
                .CountAsync();

            return Ok(new { totalPedidos = total });
        }

        // 3️⃣ Ventas totales (solo pedidos completados)
        [HttpGet("ventas-totales")]
        public async Task<IActionResult> VentasTotales()
        {
            var total = await _context.Pedidos
                .Where(p => p.Estado == "Listo")
                .SumAsync(p => (decimal?)p.Total) ?? 0;

            return Ok(new { ventasTotales = total });
        }

        // 4️⃣ Productos más vendidos
        [HttpGet("productos-mas-vendidos")]
        public async Task<IActionResult> ProductosMasVendidos()
        {
            var productos = await _context.DetallePedidos
                .Where(d => d.IdProductoNavigation.Estatus == true)
                .GroupBy(d => new { d.IdProducto, d.IdProductoNavigation.NombreProducto })
                .Select(g => new
                {
                    producto = g.Key.NombreProducto,
                    totalVendidos = g.Sum(x => x.Cantidad)
                })
                .OrderByDescending(x => x.totalVendidos)
                .Take(5)
                .ToListAsync();

            return Ok(productos);
        }

        // 5️⃣ Pedidos por día (solo completados)
        [HttpGet("pedidos-pendientes")]
        public async Task<IActionResult> PedidosPendientes()
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.Estado == "Pendiente")
                .GroupBy(p => p.FechaPedido.Date)
                .Select(g => new
                {
                    fecha = g.Key,
                    total = g.Count()
                })
                .OrderBy(g => g.fecha)
                .ToListAsync();

            return Ok(pedidos);
        }


        [HttpGet("pedidos-listos")]
        public async Task<IActionResult> PedidosListos()
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.Estado == "Listo")
                .GroupBy(p => p.FechaPedido.Date)
                .Select(g => new
                {
                    fecha = g.Key,
                    total = g.Count()
                })
                .OrderBy(g => g.fecha)
                .ToListAsync();

            return Ok(pedidos);
        }

        // 6️⃣ Total clientes ACTIVOS
        [HttpGet("total-clientes")]
        public async Task<IActionResult> TotalClientes()
        {
            var total = await _context.Clientes
                .Where(c => c.Estatus == true)
                .CountAsync();

            return Ok(new { totalClientes = total });
        }
    }
}