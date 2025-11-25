using PlataformaMulticanalFrontend.Models;
using System.Text;
using System.Text.Json;

namespace PlataformaMulticanalFrontend.Services
{
    public class CatalogoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly ILogger<CatalogoService> _logger;

        public CatalogoService(HttpClient httpClient, IConfiguration configuration, ILogger<CatalogoService> logger)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:CatalogoApiUrl"] ?? "http://localhost:8080/api/catalogo";
            _logger = logger;
        }

        // Configuración de opciones JSON
        private JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Obtener todos los productos
        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los productos desde {ApiUrl}/all", _apiUrl);
                
                var response = await _httpClient.GetAsync($"{_apiUrl}/all");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogWarning("La respuesta del servidor está vacía");
                        return new List<Producto>();
                    }
                    
                    var productos = JsonSerializer.Deserialize<List<Producto>>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Se obtuvieron {Count} productos exitosamente", productos?.Count ?? 0);
                    return productos ?? new List<Producto>();
                }
                
                _logger.LogWarning("Error al obtener productos. Status code: {StatusCode}", response.StatusCode);
                return new List<Producto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener productos");
                throw new Exception("No se pudo conectar con el servidor. Verifica que el servicio esté activo.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar la respuesta JSON");
                throw new Exception("Error al procesar la respuesta del servidor.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener productos");
                throw new Exception("Error inesperado al obtener los productos.", ex);
            }
        }

        // Obtener producto por ID
        public async Task<Producto?> ObtenerPorIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("Se intentó obtener un producto con ID nulo o vacío");
                    return null;
                }

                _logger.LogInformation("Obteniendo producto con ID: {Id}", id);
                
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var producto = JsonSerializer.Deserialize<Producto>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Producto {Id} obtenido exitosamente", id);
                    return producto;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Producto con ID {Id} no encontrado", id);
                    return null;
                }
                
                _logger.LogWarning("Error al obtener producto {Id}. Status code: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {Id}", id);
                throw new Exception($"Error al obtener el producto: {ex.Message}", ex);
            }
        }

        // Obtener productos por categoría
        public async Task<List<Producto>> ObtenerPorCategoriaAsync(string categoria)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoria))
                {
                    return await ObtenerTodosAsync();
                }

                categoria = categoria.ToLower();
                
                _logger.LogInformation("Obteniendo productos de la categoría: {Categoria}", categoria);
                
                var response = await _httpClient.GetAsync($"{_apiUrl}/categoria/{categoria}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productos = JsonSerializer.Deserialize<List<Producto>>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Se obtuvieron {Count} productos de la categoría {Categoria}", 
                        productos?.Count ?? 0, categoria);
                    return productos ?? new List<Producto>();
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("No hay productos en la categoría {Categoria}", categoria);
                    return new List<Producto>();
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("Categoría inválida: {Categoria}", categoria);
                    return new List<Producto>();
                }
                
                _logger.LogWarning("Error al obtener productos por categoría. Status code: {StatusCode}", 
                    response.StatusCode);
                return new List<Producto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al filtrar por categoría: {Categoria}", categoria);
                throw new Exception($"Error al obtener productos por categoría: {ex.Message}", ex);
            }
        }

        // Obtener productos por proveedor
        public async Task<List<Producto>> ObtenerPorProveedorAsync(string proveedorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(proveedorId))
                {
                    _logger.LogWarning("Se intentó obtener productos con proveedorId nulo o vacío");
                    return new List<Producto>();
                }

                _logger.LogInformation("Obteniendo productos del proveedor: {ProveedorId}", proveedorId);
                
                var response = await _httpClient.GetAsync($"{_apiUrl}/proveedor/{proveedorId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productos = JsonSerializer.Deserialize<List<Producto>>(content, GetJsonOptions());
                    
                    _logger.LogInformation("Se obtuvieron {Count} productos del proveedor {ProveedorId}", 
                        productos?.Count ?? 0, proveedorId);
                    return productos ?? new List<Producto>();
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("No hay productos del proveedor {ProveedorId}", proveedorId);
                    return new List<Producto>();
                }
                
                _logger.LogWarning("Error al obtener productos por proveedor. Status code: {StatusCode}", 
                    response.StatusCode);
                return new List<Producto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por proveedor: {ProveedorId}", proveedorId);
                throw new Exception($"Error al obtener productos del proveedor: {ex.Message}", ex);
            }
        }

        // Buscar productos
        public async Task<List<Producto>> BuscarAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return await ObtenerTodosAsync();
                }

                _logger.LogInformation("Buscando productos con query: {Query}", query);
                
                var productos = await ObtenerTodosAsync();
                var queryLower = query.ToLower();
                
                var resultados = productos.Where(p => 
                    p.Nombre.ToLower().Contains(queryLower) || 
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(queryLower)) ||
                    (p.Categoria != null && p.Categoria.ToLower().Contains(queryLower)) ||
                    (p.Id != null && p.Id.ToLower().Contains(queryLower))
                ).ToList();
                
                _logger.LogInformation("Búsqueda completada. Se encontraron {Count} resultados", resultados.Count);
                return resultados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con query: {Query}", query);
                throw new Exception($"Error al buscar productos: {ex.Message}", ex);
            }
        }

        // Crear producto
        public async Task<Producto?> CrearAsync(Producto producto)
        {
            try
            {
                if (producto == null)
                {
                    throw new ArgumentNullException(nameof(producto), "El producto no puede ser nulo");
                }

                _logger.LogInformation("Creando nuevo producto: {Nombre}", producto.Nombre);
                
                var json = JsonSerializer.Serialize(producto, GetJsonOptions());
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_apiUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var nuevoProducto = JsonSerializer.Deserialize<Producto>(responseContent, GetJsonOptions());
                    
                    _logger.LogInformation("Producto creado exitosamente con ID: {Id}", nuevoProducto?.Id);
                    return nuevoProducto;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear producto. Status code: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                
                throw new Exception($"Error al crear el producto. Código: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al crear producto");
                throw new Exception("No se pudo conectar con el servidor para crear el producto.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto: {Nombre}", producto?.Nombre);
                throw new Exception($"Error al crear el producto: {ex.Message}", ex);
            }
        }

        // Actualizar producto
        public async Task<Producto?> ActualizarAsync(string id, Producto producto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("El ID del producto no puede ser nulo o vacío", nameof(id));
                }

                if (producto == null)
                {
                    throw new ArgumentNullException(nameof(producto), "El producto no puede ser nulo");
                }

                _logger.LogInformation("Actualizando producto con ID: {Id}", id);
                
                var json = JsonSerializer.Serialize(producto, GetJsonOptions());
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var productoActualizado = JsonSerializer.Deserialize<Producto>(responseContent, GetJsonOptions());
                    
                    _logger.LogInformation("Producto {Id} actualizado exitosamente", id);
                    return productoActualizado;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Producto con ID {Id} no encontrado para actualizar", id);
                    return null;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al actualizar producto {Id}. Status code: {StatusCode}, Error: {Error}", 
                    id, response.StatusCode, errorContent);
                
                throw new Exception($"Error al actualizar el producto. Código: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {Id}", id);
                throw new Exception($"Error al actualizar el producto: {ex.Message}", ex);
            }
        }

        // Eliminar producto
        public async Task<bool> EliminarAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("El ID del producto no puede ser nulo o vacío", nameof(id));
                }

                _logger.LogInformation("Eliminando producto con ID: {Id}", id);
                
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Producto {Id} eliminado exitosamente", id);
                    return true;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Producto con ID {Id} no encontrado para eliminar", id);
                    return false;
                }
                
                _logger.LogWarning("Error al eliminar producto {Id}. Status code: {StatusCode}", 
                    id, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {Id}", id);
                throw new Exception($"Error al eliminar el producto: {ex.Message}", ex);
            }
        }

        // Obtener categorías únicas
        public async Task<List<string>> ObtenerCategoriasAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de categorías únicas");
                
                var productos = await ObtenerTodosAsync();
                var categorias = productos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Categoria))
                    .Select(p => p.Categoria!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
                
                _logger.LogInformation("Se encontraron {Count} categorías únicas", categorias.Count);
                return categorias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return new List<string>();
            }
        }
    }
}