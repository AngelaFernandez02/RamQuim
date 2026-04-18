namespace RamQuim.Models
{
    public class ProductoCreateDTO
    {
        public string NombreProducto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Presentacion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public IFormFile? Imagen { get; set; }
    }

}
