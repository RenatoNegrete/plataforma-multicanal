using System.Text;
using System.Text.Json;
using PlataformaMulticanalFrontend.Models;

namespace PlataformaMulticanalFrontend.Services
{
    public class ProveedorService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<ProveedorService> _logger;

        public ProveedorService(HttpClient httpClient, IConfiguration configuration, ILogger<ProveedorService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:ProveedoresUrl"] ?? "http://localhost:8080/api/proveedores";
            
            _logger.LogInformation($"ProveedorService inicializado con URL: {_baseUrl}");
        }
        
        // Configuración de opciones JSON
        private JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        // Obtener todos los proveedores
        public async Task<List<Proveedor>> ObtenerTodosAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los proveedores desde {BaseUrl}", _baseUrl);
                
                var response = await _httpClient.GetAsync(_baseUrl);
                
                _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Respuesta del servidor: {Content}", content);
                    
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogWarning("La respuesta del servidor está vacía");
                        return new List<Proveedor>();
                    }
                    
                    var proveedores = JsonSerializer.Deserialize<List<Proveedor>>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Se obtuvieron {Count} proveedores exitosamente", proveedores?.Count ?? 0);
                    return proveedores ?? new List<Proveedor>();
                }
                
                _logger.LogWarning("Error al obtener proveedores. Status code: {StatusCode}", response.StatusCode);
                return new List<Proveedor>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener proveedores");
                return new List<Proveedor>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar la respuesta JSON");
                return new List<Proveedor>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener proveedores");
                return new List<Proveedor>();
            }
        }

        // Obtener proveedor por ID
        public async Task<Proveedor?> ObtenerPorIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Obteniendo proveedor con ID: {Id}", id);
                
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var proveedor = JsonSerializer.Deserialize<Proveedor>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Proveedor {Id} obtenido exitosamente", id);
                    return proveedor;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Proveedor con ID {Id} no encontrado", id);
                    return null;
                }
                
                _logger.LogWarning("Error al obtener proveedor {Id}. Status code: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedor con ID: {Id}", id);
                return null;
            }
        }

        // Crear proveedor
        public async Task<Proveedor?> CrearAsync(Proveedor proveedor)
        {
            try
            {
                if (proveedor == null)
                {
                    throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo");
                }

                _logger.LogInformation("Creando nuevo proveedor: {Nombre}", proveedor.Nombre);
                
                var json = JsonSerializer.Serialize(proveedor, GetJsonOptions());
                _logger.LogInformation("JSON enviado: {Json}", json);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_baseUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var nuevoProveedor = JsonSerializer.Deserialize<Proveedor>(responseContent, GetJsonOptions());
                    
                    _logger.LogInformation("Proveedor creado exitosamente con ID: {Id}", nuevoProveedor?.Id);
                    return nuevoProveedor;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear proveedor. Status code: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proveedor: {Nombre}", proveedor?.Nombre);
                return null;
            }
        }

        // Actualizar proveedor
        public async Task<Proveedor?> ActualizarAsync(long id, Proveedor proveedor)
        {
            try
            {
                if (proveedor == null)
                {
                    throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo");
                }

                _logger.LogInformation("Actualizando proveedor con ID: {Id}", id);
                
                var json = JsonSerializer.Serialize(proveedor, GetJsonOptions());
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var proveedorActualizado = JsonSerializer.Deserialize<Proveedor>(responseContent, GetJsonOptions());
                    
                    _logger.LogInformation("Proveedor {Id} actualizado exitosamente", id);
                    return proveedorActualizado;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Proveedor con ID {Id} no encontrado para actualizar", id);
                    return null;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al actualizar proveedor {Id}. Status code: {StatusCode}, Error: {Error}", 
                    id, response.StatusCode, errorContent);
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proveedor con ID: {Id}", id);
                return null;
            }
        }

        // Eliminar proveedor
        public async Task<bool> EliminarAsync(long id)
        {
            try
            {
                _logger.LogInformation("Eliminando proveedor con ID: {Id}", id);
                
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Proveedor {Id} eliminado exitosamente", id);
                    return true;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Proveedor con ID {Id} no encontrado para eliminar", id);
                    return false;
                }
                
                _logger.LogWarning("Error al eliminar proveedor {Id}. Status code: {StatusCode}", 
                    id, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor con ID: {Id}", id);
                return false;
            }
        }

        // Obtener productos de un proveedor
        public async Task<List<ProductoProveedor>> ObtenerProductosProveedorAsync(long id)
        {
            try
            {
                _logger.LogInformation("Obteniendo productos del proveedor {Id}", id);
                
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}/productos");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Productos recibidos: {Content}", content);
                    
                    var productos = JsonSerializer.Deserialize<List<ProductoProveedor>>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Se obtuvieron {Count} productos del proveedor {Id}", 
                        productos?.Count ?? 0, id);
                    return productos ?? new List<ProductoProveedor>();
                }
                
                _logger.LogWarning("Error al obtener productos del proveedor {Id}. Status code: {StatusCode}", 
                    id, response.StatusCode);
                return new List<ProductoProveedor>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos del proveedor {Id}", id);
                return new List<ProductoProveedor>();
            }
        }

        // Calcular estadísticas de proveedores
        public async Task<ProveedorEstadisticas> ObtenerEstadisticasAsync()
        {
            try
            {
                var proveedores = await ObtenerTodosAsync();
                
                return new ProveedorEstadisticas
                {
                    TotalProveedores = proveedores.Count,
                    ProveedoresActivos = proveedores.Count,
                    ProveedoresPendientes = 0,
                    RatingPromedio = 4.5
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular estadísticas");
                return new ProveedorEstadisticas();
            }
        }
    }

    // Modelos auxiliares
    public class ProveedorEstadisticas
    {
        public int TotalProveedores { get; set; }
        public int ProveedoresActivos { get; set; }
        public int ProveedoresPendientes { get; set; }
        public double RatingPromedio { get; set; }
    }

    public class ProductoProveedor
    {
        public string? ProveedorId { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        public string? Descripcion { get; set; }
        public string? Categoria { get; set; }
        public int Stock { get; set; }
        public string? Error { get; set; }
        public string? Detalle { get; set; }
    }
}