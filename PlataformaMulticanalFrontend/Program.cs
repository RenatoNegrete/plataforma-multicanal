using PlataformaMulticanalFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

// ⭐ NUEVO: Registrar el servicio de catálogo
builder.Services.AddHttpClient<CatalogoService>();
builder.Services.AddScoped<CatalogoService>();

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