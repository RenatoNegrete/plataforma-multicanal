using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlataformaMulticanalFrontend.Services
{
    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:8080/auth";

        public UsuarioApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ApiResponse<FirebaseAuthResponse>> RegistrarUsuario(RegistroDto request)
        {
            try
            {
                var requestData = new
                {
                    rol = request.Rol,
                    email = request.Email,
                    password = request.Password,
                    nombre = request.Nombre,
                    telefono = request.Telefono,
                    direccion = request.Direccion
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{BaseUrl}/register", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var firebaseResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(
                        responseBody, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    
                    return new ApiResponse<FirebaseAuthResponse>
                    {
                        Success = true,
                        Data = firebaseResponse,
                        Message = "Usuario registrado exitosamente"
                    };
                }
                else
                {
                    var errorMessage = "Error al registrar usuario";
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
                        if (errorResponse != null && errorResponse.ContainsKey("message"))
                        {
                            errorMessage = errorResponse["message"].ToString();
                        }
                    }
                    catch { }

                    return new ApiResponse<FirebaseAuthResponse>
                    {
                        Success = false,
                        ErrorMessage = errorMessage
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse<FirebaseAuthResponse>
                {
                    Success = false,
                    ErrorMessage = $"Error de conexión con el servidor: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FirebaseAuthResponse>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<FirebaseAuthResponse>> LoginUsuario(LoginDto request)
        {
            try
            {
                var requestData = new
                {
                    email = request.Email,
                    password = request.Password
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{BaseUrl}/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var firebaseResponse = JsonSerializer.Deserialize<FirebaseAuthResponse>(
                        responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    
                    return new ApiResponse<FirebaseAuthResponse>
                    {
                        Success = true,
                        Data = firebaseResponse,
                        Message = "Login exitoso"
                    };
                }
                else
                {
                    return new ApiResponse<FirebaseAuthResponse>
                    {
                        Success = false,
                        ErrorMessage = "Credenciales inválidas"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse<FirebaseAuthResponse>
                {
                    Success = false,
                    ErrorMessage = $"Error de conexión con el servidor: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FirebaseAuthResponse>
                {
                    Success = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }
    }

    // DTO para el registro
    public class RegistroDto
    {
        public string Rol { get; set; } = "usuario";
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string? Url { get; set; }
    }

    // DTO para login
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Respuesta de Firebase (debe coincidir con lo que devuelve tu backend)
    public class FirebaseAuthResponse
    {
        public string IdToken { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresIn { get; set; }
        public string LocalId { get; set; }
    }

    // Respuesta genérica de la API
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }
}