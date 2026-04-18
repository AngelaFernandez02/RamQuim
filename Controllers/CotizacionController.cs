using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;
using MailKit.Net.Smtp;
using MimeKit;

[Route("api/[controller]")]
[ApiController]
public class CotizacionController : ControllerBase
{
    private readonly RamQuimContext _context;

    public CotizacionController(RamQuimContext context)
    {
        _context = context;
    }

    public class CotizacionRequest
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public List<DetalleRequest> Detalle { get; set; }
    }

    public class DetalleRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    [HttpPost("CrearCotizacion")]
    public async Task<IActionResult> CrearCotizacion([FromBody] CotizacionRequest request)
    {
        if (request.Detalle == null || !request.Detalle.Any())
            return BadRequest("Debe incluir al menos un producto.");

        var cotizacion = new Cotizacione
        {
            FechaCotizacion = DateTime.Now,
            Nombre = request.Nombre,
            Correo = request.Correo,
            Telefono = request.Telefono,
            Total = 0,
            DetalleCotizacions = new List<DetalleCotizacion>()
        };

        decimal total = 0;
        var cuerpoCorreo = $"Hola {request.Nombre},\n\nGracias por tu solicitud. Aquí está tu cotización:\n\n";

        foreach (var item in request.Detalle)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == item.IdProducto);

            if (producto == null)
                return BadRequest($"Producto con Id {item.IdProducto} no existe.");

            var subtotal = producto.PrecioUnitario * item.Cantidad;

            cotizacion.DetalleCotizacions.Add(new DetalleCotizacion
            {
                IdProducto = item.IdProducto,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.PrecioUnitario,
                Subtotal = subtotal
            });

            total += subtotal;

            cuerpoCorreo += $" Nombre producto : {producto.NombreProducto}: Cantidad: {item.Cantidad} x {producto.PrecioUnitario:C} = {subtotal:C}\n";
        }

        cotizacion.Total = total;
        _context.Cotizaciones.Add(cotizacion);
        await _context.SaveChangesAsync();

        cuerpoCorreo += $"\nTotal: {total:C}\n\nSaludos,\nRamQuim";

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RamQuim", "mrcookiesoftware@gmail.com"));
            message.To.Add(MailboxAddress.Parse(request.Correo));
            message.Subject = "Cotización RamQuim";
            message.Body = new TextPart("plain") { Text = cuerpoCorreo };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync("mrcookiesoftware@gmail.com", "ltpbgcwchapmbipu");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                mensaje = "Cotización creada correctamente, pero el correo no se pudo enviar.",
                errorCorreo = ex.Message
            });
        }

       
        return Ok(new
        {
            idCotizacion = cotizacion.IdCotizacion,
            fechaCotizacion = cotizacion.FechaCotizacion,
            total = cotizacion.Total,
            nombre = cotizacion.Nombre,
            correo = cotizacion.Correo,
            telefono = cotizacion.Telefono,
            detalle = cotizacion.DetalleCotizacions.Select(d => new
            {
                idProducto = d.IdProducto,
                cantidad = d.Cantidad,
                precioUnitario = d.PrecioUnitario,
                subtotal = d.Subtotal
            })
        });
    }
}