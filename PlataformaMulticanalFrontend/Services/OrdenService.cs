using PlataformaMulticanalFrontend.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlataformaMulticanalFrontend.Services
{
    public class OrdenService
    {
        private readonly HttpClient _httpClient;
        private readonly string _ordenesApiUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrdenService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            
            // Intenta obtener la URL específica, si no existe usa la base
            _ordenesApiUrl = configuration["ApiSettings:OrdenesApiUrl"] 
                ?? $"{configuration["ApiSettings:BaseUrl"]}/api/ordenes"
                ?? "http://localhost:8080/api/ordenes";
            
            // Configuración de JSON para manejar fechas correctamente
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<Orden?> CrearOrdenAsync(Orden orden)
        {
            try
            {
                var json = JsonSerializer.Serialize(orden, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_ordenesApiUrl, content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Orden>(responseContent, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión al crear orden: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear orden: {ex.Message}", ex);
            }
        }

        public async Task<List<Orden>> ListarOrdenesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_ordenesApiUrl);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Orden>>(content, _jsonOptions) ?? new List<Orden>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión al listar órdenes: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar órdenes: {ex.Message}", ex);
            }
        }

        public async Task<Orden?> ObtenerOrdenPorIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ordenesApiUrl}/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Orden>(content, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión al obtener orden: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener orden: {ex.Message}", ex);
            }
        }

        public async Task<List<Orden>> ListarOrdenesPorClienteAsync(long clienteId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ordenesApiUrl}/cliente/{clienteId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Orden>>(content, _jsonOptions) ?? new List<Orden>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión al listar órdenes del cliente: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar órdenes del cliente: {ex.Message}", ex);
            }
        }

        public async Task<OrdenResponse> CrearOrdenTestAsync(CrearOrdenDto ordenDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(ordenDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"[DEBUG] Creando orden: {json}");
                
                var response = await _httpClient.PostAsync(_ordenesApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[DEBUG] Response Status: {response.StatusCode}");
                Console.WriteLine($"[DEBUG] Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return new OrdenResponse
                    {
                        Success = true,
                        Message = "Orden creada exitosamente",
                        Data = responseContent
                    };
                }
                else
                {
                    return new OrdenResponse
                    {
                        Success = false,
                        Message = $"Error al crear la orden: {response.StatusCode}",
                        ErrorMessage = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception al crear orden: {ex.Message}");
                return new OrdenResponse
                {
                    Success = false,
                    Message = "Error al conectar con el servidor",
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    // DTOs
    public class CrearOrdenDto
    {
        [JsonPropertyName("clienteId")]
        public int ClienteId { get; set; }

        [JsonPropertyName("productos")]
        public List<ProductoOrdenDto> Productos { get; set; } = new List<ProductoOrdenDto>();

        [JsonPropertyName("clienteMail")]
        public string ClienteMail { get; set; } = string.Empty;
    }

    public class ProductoOrdenDto
    {
        [JsonPropertyName("productoId")]
        public string ProductoId { get; set; } = string.Empty;

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }
    }

    public class OrdenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}