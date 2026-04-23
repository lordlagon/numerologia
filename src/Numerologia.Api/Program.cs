using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Interfaces;
using Numerologia.Core.Services;
using Numerologia.Infrastructure.Data;
using Numerologia.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// EF Core + PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// DI
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? "";
    options.Events.OnTicketReceived = async context =>
    {
        var usuarioService = context.HttpContext.RequestServices.GetRequiredService<UsuarioService>();

        var googleId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email = context.Principal?.FindFirstValue(ClaimTypes.Email) ?? "";
        var nome = context.Principal?.FindFirstValue(ClaimTypes.Name) ?? "";

        await usuarioService.ObterOuCriarAsync(googleId, email, nome);
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Railway termina HTTPS no proxy — não redirecionar internamente
// app.UseHttpsRedirection();

// Blazor WASM usa extensões que o ASP.NET Core não reconhece por padrão (.dat, .blat)
var contentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".dat"]  = "application/octet-stream";
contentTypeProvider.Mappings[".blat"] = "application/octet-stream";
contentTypeProvider.Mappings[".wasm"] = "application/wasm";

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider,
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream"
});

app.UseAuthentication();
app.UseAuthorization();

// Endpoints

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.MapGet("/auth/login", () =>
    Results.Challenge(new AuthenticationProperties { RedirectUri = "/" },
        [GoogleDefaults.AuthenticationScheme]));

app.MapGet("/auth/me", async (HttpContext context, UsuarioService usuarioService) =>
{
    var googleId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = context.User.FindFirstValue(ClaimTypes.Email);
    var nome = context.User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(googleId))
        return Results.Unauthorized();

    var usuario = await usuarioService.ObterOuCriarAsync(googleId, email ?? "", nome ?? "");

    return Results.Ok(new { usuario.Email, usuario.Nome });
}).RequireAuthorization();

app.MapPost("/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok(new { message = "logout" });
});

// Fallback para o roteamento client-side do Blazor
app.MapFallbackToFile("index.html");

app.Run();

// Necessário para WebApplicationFactory nos testes de integração
public partial class Program { }
