using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlataformaMulticanalFrontend.Models
{
    public class Proveedor
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Teléfono inválido")]
        [JsonPropertyName("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La URL es requerida")]
        [Url(ErrorMessage = "URL inválida")]
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}