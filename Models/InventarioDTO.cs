namespace RamQuim.Models
{
    public class InventarioDTO
    {

   
        public int IdInventario { get; set; }

        public int IdProducto { get; set; }


        public decimal CantidadDisponible { get; set; }
        public virtual Producto IdProductoNavigation { get; set; } = null!;
 public string nombreProducto { get; set; }
        public string presentacion { get; set; }
        public string descripcion { get; set; }
        public string imagen { get; set; }
    }
}
