using PlataformaMulticanalFrontend.Models;
using System.Text;
using System.Text.Json;

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
    }
}