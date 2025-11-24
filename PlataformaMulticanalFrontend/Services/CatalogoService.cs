using PlataformaMulticanalFrontend.Models;
using System.Text;
using System.Text.Json;

namespace PlataformaMulticanalFrontend.Services
{
    public class CatalogoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public CatalogoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:CatalogoApiUrl"] ?? "http://localhost:8080/api/catalogo";
        }

        // Obtener todos los productos
        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/all");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var productos = JsonSerializer.Deserialize<List<Producto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return productos ?? new List<Producto>();
                }
                
                return new List<Producto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Obtener producto por ID
        public async Task<Producto?> ObtenerPorIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var producto = JsonSerializer.Deserialize<Producto>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return producto;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener producto: {ex.Message}");
                return null;
            }
        }

        // Filtrar por categoría
        public async Task<List<Producto>> FiltrarPorCategoriaAsync(string categoria)
        {
            try
            {
                var productos = await ObtenerTodosAsync();
                return productos.Where(p => p.Categoria?.ToLower() == categoria.ToLower()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por categoría: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Buscar productos
        public async Task<List<Producto>> BuscarAsync(string query)
        {
            try
            {
                var productos = await ObtenerTodosAsync();
                var queryLower = query.ToLower();
                
                return productos.Where(p => 
                    p.Nombre.ToLower().Contains(queryLower) || 
                    (p.Descripcion != null && p.Descripcion.ToLower().Contains(queryLower)) ||
                    (p.Categoria != null && p.Categoria.ToLower().Contains(queryLower))
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar productos: {ex.Message}");
                return new List<Producto>();
            }
        }

        // Crear producto
        public async Task<Producto?> CrearAsync(Producto producto)
        {
            try
            {
                var json = JsonSerializer.Serialize(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_apiUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Producto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear producto: {ex.Message}");
                return null;
            }
        }

        // Actualizar producto
        public async Task<Producto?> ActualizarAsync(string id, Producto producto)
        {
            try
            {
                var json = JsonSerializer.Serialize(producto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Producto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto: {ex.Message}");
                return null;
            }
        }

        // Eliminar producto
        public async Task<bool> EliminarAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                return false;
            }
        }
    }
}