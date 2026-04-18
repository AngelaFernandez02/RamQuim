using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

namespace RamQuim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly RamQuimContext _baseDatos;

        public RolController(RamQuimContext basesDatos)
        {
            _baseDatos = basesDatos;
        }


        [HttpGet]
        [Route("ListaRoles")]

        public async Task<IActionResult> Lista()
        {
            var listaRol = await _baseDatos.Roles.ToListAsync();
            return Ok(listaRol);
        }

        [HttpPost]
        [Route("AgregarRol")]
        public async Task<IActionResult> Agregar([FromBody] Role request)
        {
            await _baseDatos.Roles.AddAsync(request);
            await _baseDatos.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut]
        [Route("ModificarRol/{idRol:int}")]
        public async Task<IActionResult> Modificar(int idRol, [FromBody] Role request)
        {
            var rolModificar = await _baseDatos.Roles.FindAsync(idRol);

            if (rolModificar == null)
            {
                return NotFound("No existe el rol");
            }
            rolModificar.NombreRol = request.NombreRol;

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(rolModificar);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al actualizar el rol");
            }

        }

        [HttpDelete]
        [Route("EliminarRol/{idRol:int}")]

        public async Task<IActionResult> Eliminar(int idRol)
        {
            var rolEliminar = await _baseDatos.Roles.FindAsync(idRol);
            if (rolEliminar == null)
            {
                return NotFound("No existe el rol");
            }

            _baseDatos.Roles.Remove(rolEliminar);

            try
            {
                await _baseDatos.SaveChangesAsync();
                return Ok(new { Mensaje = "ROL eliminado correctamente" });

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Error al eliminar el ROL");
            }
        }
    }
}