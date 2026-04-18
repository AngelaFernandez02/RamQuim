using System;
using System.Collections.Generic;

namespace RamQuim.Models;

public partial class Cotizacione
{
    public int IdCotizacion { get; set; }

    public DateTime FechaCotizacion { get; set; }

    public decimal? Total { get; set; }


    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }


    public virtual ICollection<DetalleCotizacion> DetalleCotizacions { get; set; } = new List<DetalleCotizacion>();
}
