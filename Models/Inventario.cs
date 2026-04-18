using System;
using System.Collections.Generic;

namespace RamQuim.Models;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int IdProducto { get; set; }
   


    public decimal CantidadDisponible { get; set; }

    public string? UnidadMedida { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
