using Microsoft.AspNetCore.Mvc;
using PlataformaMulticanalFrontend.Services;
using System.Threading.Tasks;

namespace PlataformaMulticanalFrontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioApiService _apiService;
        private readonly PerfilApiService _perfilService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UsuarioApiService apiService, 
            PerfilApiService perfilService,
            ILogger<AccountController> logger)
        {
            _apiService = apiService;
            _perfilService = perfilService;
            _logger = logger;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewData["Title"] = "Iniciar Sesión";
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string userRole, string? returnUrl)
        {
            _logger.LogInformation($"=== INICIO LOGIN === Email: {email}, Rol: {userRole}");

            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Por favor completa todos los campos";
                return View();
            }

            // Validar que se haya seleccionado un rol
            if (string.IsNullOrWhiteSpace(userRole))
            {
                TempData["Error"] = "Por favor selecciona un tipo de cuenta";
                return View();
            }

            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            // PASO 1: Login con Firebase
            var response = await _apiService.LoginUsuario(loginDto);

            if (!response.Success)
            {
                _logger.LogWarning($"❌ Error en login Firebase: {response.ErrorMessage}");
                TempData["Error"] = response.ErrorMessage ?? "Error al iniciar sesión";
                return View();
            }

            _logger.LogInformation($"✅ Login Firebase exitoso para: {email}");

            // PASO 2: Guardar información de Firebase en la sesión
            HttpContext.Session.SetString("FirebaseToken", response.Data.IdToken);
            HttpContext.Session.SetString("FirebaseUserId", response.Data.LocalId);
            HttpContext.Session.SetString("UserEmail", response.Data.Email);
            HttpContext.Session.SetString("RefreshToken", response.Data.RefreshToken);
            HttpContext.Session.SetString("UserRole", userRole);

            _logger.LogInformation($"💾 Sesión Firebase guardada - Email: {response.Data.Email}");

            // PASO 3: Obtener datos del usuario desde la BD
            _logger.LogInformation($"🔍 Buscando usuario en BD con email: {response.Data.Email}");
            
            var userResponse = await _perfilService.ObtenerPorEmail(response.Data.Email);
            
            if (userResponse.Success && userResponse.Data != null)
            {
                // Guardar datos del usuario en sesión
                HttpContext.Session.SetString("UserId", userResponse.Data.Id.ToString());
                HttpContext.Session.SetString("UserName", userResponse.Data.Nombre ?? "Usuario");
                
                _logger.LogInformation($"✅ Usuario encontrado en BD - ID: {userResponse.Data.Id}, Nombre: {userResponse.Data.Nombre}");
                
                // Guardar teléfono si existe
                if (!string.IsNullOrEmpty(userResponse.Data.Telefono))
                {
                    HttpContext.Session.SetString("UserPhone", userResponse.Data.Telefono);
                }

                TempData["Success"] = "¡Bienvenido de nuevo!";

                // Redirigir según el rol
                switch (userRole)
                {
                    case "admin":
                        return Redirect("/admin/dashboard");
                    case "proveedor":
                        return Redirect("/proveedor/dashboard");
                    case "cliente":
                    default:
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Usuario no encontrado en BD - intentar crear uno básico
                _logger.LogWarning($"⚠️ Usuario no encontrado en BD. Intentando crear registro básico...");
                
                try
                {
                    // Crear un usuario básico en la BD usando el endpoint /usuarios/register del backend
                    var crearUsuarioDto = new RegistroDto
                    {
                        Email = response.Data.Email,
                        Password = "temp_password", // Contraseña temporal, ya tiene cuenta en Firebase
                        Nombre = email.Split('@')[0], // Usar parte del email como nombre temporal
                        Telefono = "",
                        Direccion = "",
                        Rol = userRole
                    };

                    var createResponse = await _apiService.RegistrarUsuario(crearUsuarioDto);
                    
                    if (createResponse.Success)
                    {
                        _logger.LogInformation($"✅ Usuario creado en BD exitosamente");
                        
                        // Intentar obtener el usuario nuevamente
                        var retryUserResponse = await _perfilService.ObtenerPorEmail(response.Data.Email);
                        
                        if (retryUserResponse.Success && retryUserResponse.Data != null)
                        {
                            HttpContext.Session.SetString("UserId", retryUserResponse.Data.Id.ToString());
                            HttpContext.Session.SetString("UserName", retryUserResponse.Data.Nombre);
                            
                            TempData["Warning"] = "Perfil creado. Por favor completa tu información.";
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ No se pudo crear usuario en BD: {createResponse.ErrorMessage}");
                        HttpContext.Session.SetString("UserName", email.Split('@')[0]);
                        TempData["Warning"] = "Perfil incompleto. Por favor completa tu información.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al crear usuario en BD");
                    HttpContext.Session.SetString("UserName", email.Split('@')[0]);
                    TempData["Warning"] = "Perfil incompleto. Por favor completa tu información.";
                }

                switch (userRole)
                {
                    case "admin":
                        return Redirect("/admin/dashboard");
                    case "proveedor":
                        return Redirect("/proveedor/dashboard");
                    case "cliente":
                    default:
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        return RedirectToAction("Index", "Home");
                }
            }
        }

        // GET: Account/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            ViewData["Title"] = "Crear Cuenta";
            return View();
        }

        // POST: Account/Registro
        [HttpPost]
        public async Task<IActionResult> Registro(
            string firstName, 
            string lastName, 
            string email, 
            string password,
            string telefono,
            string direccion,
            string? urlProveedor,
            string userRole = "usuario")
        {
            _logger.LogInformation($"=== INICIO REGISTRO === Email: {email}, Rol: {userRole}");

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(firstName) || 
                string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Por favor completa todos los campos obligatorios";
                return View();
            }

            if (password.Length < 6)
            {
                TempData["Error"] = "La contraseña debe tener al menos 6 caracteres";
                return View();
            }

            if (userRole == "proveedor")
            {
                if (string.IsNullOrWhiteSpace(urlProveedor))
                {
                    TempData["Error"] = "La URL del sitio web es obligatoria para proveedores";
                    return View();
                }

                // Validar formato de URL
                if (!Uri.TryCreate(urlProveedor, UriKind.Absolute, out var uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    TempData["Error"] = "Por favor ingresa una URL válida (ejemplo: https://tusitio.com)";
                    return View();
                }
            }

            var registroDto = new RegistroDto
            {
                Rol = userRole, 
                Nombre = $"{firstName} {lastName}",
                Email = email,
                Password = password,
                Telefono = telefono ?? "",
                Direccion = direccion ?? "",
                Url = userRole == "proveedor" ? urlProveedor : null,
            };

            var response = await _apiService.RegistrarUsuario(registroDto);

            if (response.Success)
            {
                _logger.LogInformation($"✅ Registro exitoso para: {email}");
                TempData["Success"] = "¡Registro exitoso! Ahora puedes iniciar sesión con tu cuenta.";
                return RedirectToAction("Login");
            }

            _logger.LogWarning($"❌ Error en registro: {response.ErrorMessage}");
            TempData["Error"] = response.ErrorMessage ?? "Error al registrar usuario";
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            _logger.LogInformation("🚪 Usuario cerrando sesión");
            HttpContext.Session.Clear();
            TempData["Success"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }
    }
}