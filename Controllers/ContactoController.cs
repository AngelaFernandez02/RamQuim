using Microsoft.AspNetCore.Mvc;
using RamQuim.Models;
using RamQuim.Services;

namespace RamQuim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly EmailService _emailService;

        public ContactoController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("enviar-contacto")]
        public async Task<IActionResult> EnviarContacto([FromBody] ContactoDto dto)
        {
            try
            {
                await _emailService.EnviarCorreoAsync(dto);
                return Ok(new { mensaje = "Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}