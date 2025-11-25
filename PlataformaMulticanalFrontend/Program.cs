using PlataformaMulticanalFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ⭐ Registrar IHttpContextAccessor para acceder a la sesión
builder.Services.AddHttpContextAccessor();

// Configurar sesiones para almacenar datos del usuario
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sesión expira en 30 minutos
    options.Cookie.HttpOnly = true; // Cookie solo accesible por HTTP (seguridad)
    options.Cookie.IsEssential = true; // Cookie esencial para la aplicación
    options.Cookie.Name = ".PlataformaMulticanal.Session"; // Nombre de la cookie
});

// Configurar HttpClient para servicios API
builder.Services.AddHttpClient();

// ⭐ Registrar servicios de la aplicación

// Servicio de catálogo
builder.Services.AddHttpClient<CatalogoService>();
builder.Services.AddScoped<CatalogoService>();

// Servicio de proveedores
builder.Services.AddHttpClient<ProveedorService>();
builder.Services.AddScoped<ProveedorService>();

builder.Services.AddHttpClient<OrdenService>();
builder.Services.AddScoped<OrdenService>();

// Servicio de usuarios y autenticación
builder.Services.AddScoped<UsuarioApiService>();

// Servicio de perfil de usuario
builder.Services.AddScoped<PerfilApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: UseSession debe ir ANTES de UseAuthorization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();