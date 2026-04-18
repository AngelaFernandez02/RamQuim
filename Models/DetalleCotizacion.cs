using System;
using System.Collections.Generic;

namespace RamQuim.Models;

public partial class DetalleCotizacion
{
    public int IdDetalleCotizacion { get; set; }

    public int IdCotizacion { get; set; }

    public int IdProducto { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Cotizacione IdCotizacionNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
