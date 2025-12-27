using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PlataformaMulticanalFrontend.Services
{
    public class PerfilApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PerfilApiService> _logger;
        private const string BaseUrl = "http://localhost:8080/usuarios";

        public PerfilApiService(IHttpClientFactory httpClientFactory, ILogger<PerfilApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _logger = logger;
        }

        // Obtener usuario por email
        public async Task<ApiResponse<UsuarioDto>> ObtenerPorEmail(string email)
        {
            try
            {
                // Limpiar y codificar el email
                var emailLimpio = email.Trim();
                var emailCodificado = Uri.EscapeDataString(emailLimpio);
                var url = $"{BaseUrl}/email/{emailCodificado}";
                
                _logger.LogInformation($"📡 GET {url}");

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📊 Status: {(int)response.StatusCode} {response.StatusCode}");
                _logger.LogInformation($"📄 Response: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    // Verificar si la respuesta está vacía
                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        _logger.LogWarning("⚠️ Respuesta vacía del servidor");
                        return new ApiResponse<UsuarioDto>
                        {
                            Success = false,
                            ErrorMessage = "El servidor retornó una respuesta vacía"
                        };
                    }

                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var usuario = JsonSerializer.Deserialize<UsuarioDto>(responseBody, options);

                    if (usuario == null)
                    {
                        _logger.LogWarning("⚠️ No se pudo deserializar el usuario");
                        return new ApiResponse<UsuarioDto>
                        {
                            Success = false,
                            ErrorMessage = "Error al procesar la respuesta del servidor"
                        };
                    }

                    _logger.LogInformation($"✅ Usuario encontrado - ID: {usuario.Id}, Email: {usuario.Email}, Nombre: {usuario.Nombre}");

                    return new ApiResponse<UsuarioDto>
                    {
                        Success = true,
                        Data = usuario,
                        Message = "Usuario obtenido exitosamente"
                    };
                }
                else
                {
                    _logger.LogWarning($"❌ HTTP {(int)response.StatusCode}: {responseBody}");
                    
                    var errorMsg = response.StatusCode == System.Net.HttpStatusCode.NotFound
                        ? "No se encontró el usuario en la base de datos"
                        : $"Error del servidor: {response.StatusCode}";

                    return new ApiResponse<UsuarioDto>
                    {
                        Success = false,
                        ErrorMessage = errorMsg
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Error de conexión HTTP");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"No se pudo conectar al servidor: {ex.Message}. Verifica que el backend esté en ejecución."
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "❌ Error al deserializar JSON");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error al procesar datos del servidor: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }

        // Obtener usuario por ID
        public async Task<ApiResponse<UsuarioDto>> ObtenerPorId(long id)
        {
            try
            {
                var url = $"{BaseUrl}/{id}";
                _logger.LogInformation($"📡 GET {url}");

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📊 Status: {(int)response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var usuario = JsonSerializer.Deserialize<UsuarioDto>(responseBody, options);

                    _logger.LogInformation($"✅ Usuario encontrado - ID: {usuario?.Id}");

                    return new ApiResponse<UsuarioDto>
                    {
                        Success = true,
                        Data = usuario,
                        Message = "Usuario obtenido exitosamente"
                    };
                }
                else
                {
                    _logger.LogWarning($"❌ Usuario no encontrado - ID: {id}");
                    return new ApiResponse<UsuarioDto>
                    {
                        Success = false,
                        ErrorMessage = "No se encontró el usuario"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Error de conexión HTTP");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error de conexión con el servidor: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }

        // Actualizar usuario
        public async Task<ApiResponse<UsuarioDto>> ActualizarUsuario(long id, UsuarioUpdateDto request)
        {
            try
            {
                var url = $"{BaseUrl}/{id}";
                _logger.LogInformation($"📡 PUT {url}");
                _logger.LogInformation($"📝 Datos: Nombre={request.Nombre}, Tel={request.Telefono}");

                var options = new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(request, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📊 Status: {(int)response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var usuario = JsonSerializer.Deserialize<UsuarioDto>(
                        responseBody,
                        new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        }
                    );

                    _logger.LogInformation($"✅ Usuario actualizado - ID: {id}");

                    return new ApiResponse<UsuarioDto>
                    {
                        Success = true,
                        Data = usuario,
                        Message = "Usuario actualizado exitosamente"
                    };
                }
                else
                {
                    _logger.LogWarning($"❌ Error al actualizar: {responseBody}");
                    return new ApiResponse<UsuarioDto>
                    {
                        Success = false,
                        ErrorMessage = "Error al actualizar el usuario"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Error de conexión HTTP");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error de conexión con el servidor: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                return new ApiResponse<UsuarioDto>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }

        // Listar todos los usuarios (para debug)
        public async Task<ApiResponse<List<UsuarioDto>>> ListarUsuarios()
        {
            try
            {
                var url = BaseUrl;
                _logger.LogInformation($"📡 GET {url}");

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📊 Status: {(int)response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var usuarios = JsonSerializer.Deserialize<List<UsuarioDto>>(
                        responseBody,
                        new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        }
                    );

                    _logger.LogInformation($"✅ Encontrados {usuarios?.Count ?? 0} usuarios");

                    return new ApiResponse<List<UsuarioDto>>
                    {
                        Success = true,
                        Data = usuarios,
                        Message = "Usuarios obtenidos exitosamente"
                    };
                }
                else
                {
                    return new ApiResponse<List<UsuarioDto>>
                    {
                        Success = false,
                        ErrorMessage = "Error al obtener los usuarios"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado");
                return new ApiResponse<List<UsuarioDto>>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }
    }

    // DTO para recibir datos del usuario desde el backend
    public class UsuarioDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }

    // DTO para actualizar usuario
    public class UsuarioUpdateDto
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}