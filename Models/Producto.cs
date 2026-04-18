using System;
using System.Collections.Generic;

namespace RamQuim.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Presentacion { get; set; }

    public decimal PrecioUnitario { get; set; }

    public string? Imagen { get; set; }

    public bool Estatus { get; set; }

    public virtual ICollection<DetalleCotizacion> DetalleCotizacions { get; set; } = new List<DetalleCotizacion>();

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
}
