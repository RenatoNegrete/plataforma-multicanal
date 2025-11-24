namespace PlataformaMulticanalFrontend.Models
{
    public class Producto
    {
        public string? Id { get; set; }
        public string? ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Precio { get; set; }
        public string? Imagen { get; set; }
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public int Stock { get; set; }
    }
}