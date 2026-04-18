using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;
using BCrypt.Net;

namespace RamQuim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly RamQuimContext _baseDatos;

        public UsuarioController(RamQuimContext basesDatos)
        {
            _baseDatos = basesDatos;
        }

        // ================= LISTAR USUARIOS =================
        [HttpGet]
        [Route("ListaUsuarios")]
        public async Task<IActionResult> Lista()
        {
            var listaUsuario = await _baseDatos.Usuarios.ToListAsync();
            return Ok(listaUsuario);
        }

        // ================= AGREGAR USUARIO =================
        [HttpPost]
        [Route("AgregarUsuario")]
        public async Task<IActionResult> Agregar([FromBody] Usuario request)
        {
            if (string.IsNullOrWhiteSpace(request.Usuario1) || string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return BadRequest(new { mensaje = "Usuario y contraseña son obligatorios" });
            }

            // Verificar si el usuario ya existe
            var usuarioExiste = await _baseDatos.Usuarios
                .AnyAsync(u => u.Usuario1 == request.Usuario1);

            if (usuarioExiste)
            {
                return BadRequest(new { mensaje = "El usuario ya existe" });
            }

            // Hashear contraseña
            request.Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);

            await _baseDatos.Usuarios.AddAsync(request);
            await _baseDatos.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario registrado correctamente",
                usuario = request
            });
        }

        // ================= MODIFICAR USUARIO =================
        [HttpPut]
        [Route("ModificarUsuario/{idUsuario:int}")]
        public async Task<IActionResult> Modificar(int idUsuario, [FromBody] Usuario request)
        {
            var usuarioModificar = await _baseDatos.Usuarios.FindAsync(idUsuario);

            if (usuarioModificar == null)
                return NotFound(new { mensaje = "No existe el usuario" });

            usuarioModificar.Usuario1 = request.Usuario1;

            // Solo cambiar contraseña si viene nueva
            if (!string.IsNullOrWhiteSpace(request.Contrasena))
            {
                usuarioModificar.Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena);
            }

            usuarioModificar.IdRol = request.IdRol;
            usuarioModificar.Estatus = request.Estatus;
            usuarioModificar.IdCliente = request.IdCliente;

            await _baseDatos.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario actualizado correctamente",
                usuario = usuarioModificar
            });
        }

        // ================= ELIMINAR USUARIO =================
        [HttpDelete]
        [Route("EliminarUsuario/{idUsuario:int}")]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            var usuarioEliminar = await _baseDatos.Usuarios.FindAsync(idUsuario);

            if (usuarioEliminar == null)
                return NotFound(new { mensaje = "No existe el usuario" });

            _baseDatos.Usuarios.Remove(usuarioEliminar);
            await _baseDatos.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario eliminado correctamente" });
        }

        // ================= LOGIN =================
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Usuario request)
        {
            var usuario = await _baseDatos.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Usuario1 == request.Usuario1 &&
                    u.Estatus == true);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario no encontrado" });

            bool passwordValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);

            if (!passwordValida)
                return Unauthorized(new { mensaje = "Contraseña incorrecta" });

            return Ok(new
            {
                idUsuario = usuario.IdUsuario,
                usuario = usuario.Usuario1,
                rol = usuario.IdRol,
                idCliente = usuario.IdCliente ?? 0
            });
        }
    }
}