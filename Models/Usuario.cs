using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace RamQuim.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int IdRol { get; set; }

    public bool Estatus { get; set; }

    public int? IdCliente { get; set; }

    [JsonIgnore]
    public virtual Role? IdRolNavigation { get; set; }

    // AQUÍ ESTA LA CORRECCIÓN
    [ForeignKey("IdCliente")]
    [JsonIgnore]
    public virtual Cliente? IdClienteNavigation { get; set; }
}