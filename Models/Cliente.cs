using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RamQuim.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? NombreContacto { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public string? Rfc { get; set; }

    public bool Estatus { get; set; }

    [JsonIgnore]
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
