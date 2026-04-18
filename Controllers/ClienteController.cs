using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RamQuim.Models;

namespace RamQuim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly RamQuimContext _baseDatos;

        public ClienteController(RamQuimContext context)
        {
            _baseDatos = context;
        }

        //  LISTAR CLIENTES
        [HttpGet]
        [Route("ListaClientes")]
        public async Task<IActionResult> Lista()
        {
            var lista = await _baseDatos.Clientes.ToListAsync();
            return Ok(lista);
        }

        //  OBTENER CLIENTE POR ID
        [HttpGet]
        [Route("ObtenerCliente/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var cliente = await _baseDatos.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            return Ok(cliente);
        }

        //  AGREGAR CLIENTE
        [HttpPost]
        [Route("AgregarCliente")]
        public async Task<IActionResult> Agregar([FromBody] Cliente request)
        {
            await _baseDatos.Clientes.AddAsync(request);
            await _baseDatos.SaveChangesAsync();

            return Ok(request);
        }

        //  MODIFICAR CLIENTE
        [HttpPut]
        [Route("ModificarCliente/{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] Cliente request)
        {
            var cliente = await _baseDatos.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            cliente.NombreEmpresa = request.NombreEmpresa;
            cliente.NombreContacto = request.NombreContacto;
            cliente.Correo = request.Correo;
            cliente.Telefono = request.Telefono;
            cliente.Direccion = request.Direccion;
            cliente.Rfc = request.Rfc;
            cliente.Estatus = request.Estatus;

            await _baseDatos.SaveChangesAsync();

            return Ok(cliente);
        }

        //  ELIMINAR CLIENTE
        [HttpDelete]
        [Route("EliminarCliente/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cliente = await _baseDatos.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound("Cliente no encontrado");

            _baseDatos.Clientes.Remove(cliente);
            await _baseDatos.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente eliminado correctamente" });
        }
    }
}