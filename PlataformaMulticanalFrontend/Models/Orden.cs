namespace PlataformaMulticanalFrontend.Models
{
    public class Orden
    {
        public string? Id { get; set; }
        public long ClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
        public List<string>? Productos { get; set; }

        public Orden()
        {
            Productos = new List<string>();
            Fecha = DateTime.Now;
        }
    }
}