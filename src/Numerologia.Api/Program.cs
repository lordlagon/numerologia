using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NetEscapades.AspNetCore.SecurityHeaders;
using Npgsql;
using Scalar.AspNetCore;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Core.Services;
using Numerologia.Infrastructure.Data;
using Numerologia.Infrastructure.Repositories;
using MapasRepository = Numerologia.Infrastructure.Repositories.MapasRepository;

var builder = WebApplication.CreateBuilder(args);

// EF Core + PostgreSQL
// Railway injeta DATABASE_URL no formato URI (postgres://user:pass@host:port/db).
// Npgsql espera key-value — converte quando necessário.
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

var connectionString = ToNpgsqlConnectionString(rawConnectionString);

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

static string? ToNpgsqlConnectionString(string? value)
{
    if (string.IsNullOrEmpty(value)) return null;
    if (!value.StartsWith("postgres://") && !value.StartsWith("postgresql://")) return value;

    var uri = new Uri(value);
    var userInfo = uri.UserInfo.Split(':', 2);
    return new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
        SslMode = SslMode.Require,
        TrustServerCertificate = true,
    }.ConnectionString;
}

// Data Protection — persiste chaves no banco para sobreviver a redeploys no Railway
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDataProtection()
        .SetApplicationName("numerologia-crm")
        .PersistKeysToDbContext<AppDbContext>();
}

// DI
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IConsulentesRepository, ConsulentesRepository>();
builder.Services.AddScoped<IMapasRepository, MapasRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<GeradorMapa>();

// Autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // HttpOnly impede acesso via JavaScript; SameSite=Lax permite o redirect OAuth cross-origin
    // SecurePolicy=SameAsRequest funciona no Railway onde TLS é terminado no proxy (request interno é HTTP)
    options.Cookie.HttpOnly     = true;
    options.Cookie.SameSite     = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan    = TimeSpan.FromDays(30);
    options.SlidingExpiration = true; // renova o prazo a cada request
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? "";
    options.Events.OnTicketReceived = async context =>
    {
        // Marca o cookie como persistente (sobrevive ao fechamento do browser)
        context.Properties!.IsPersistent = true;
        context.Properties.ExpiresUtc   = DateTimeOffset.UtcNow.AddDays(30);

        var usuarioService = context.HttpContext.RequestServices.GetRequiredService<UsuarioService>();

        var googleId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var email = context.Principal?.FindFirstValue(ClaimTypes.Email) ?? "";
        var nome = context.Principal?.FindFirstValue(ClaimTypes.Name) ?? "";

        await usuarioService.ObterOuCriarAsync(googleId, email, nome);
    };
});

builder.Services.AddAuthorization();

// Rate Limiting — protege contra abuso e DoS
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Geral: 60 req/min por usuário autenticado (ou por IP para não-autenticados)
    options.AddPolicy("api-geral", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60, Window = TimeSpan.FromMinutes(1), QueueLimit = 0
            }));

    // Mapas: 10 req/min — operação CPU-intensiva (executa todos os cálculos numerológicos)
    options.AddPolicy("mapas", ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? ctx.Connection.RemoteIpAddress?.ToString() ?? "anon",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, Window = TimeSpan.FromMinutes(1), QueueLimit = 0
            }));
});

// OpenAPI — disponível em Development e Staging; desabilitado em Production
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, context, ct) =>
    {
        doc.Info.Title       = "Numerologia API";
        doc.Info.Version     = "v1";
        doc.Info.Description = "API do CRM de Numerologia Cabalística. Autenticação via Google OAuth (cookie). " +
                               "Para testar endpoints protegidos, faça login em /auth/login antes de usar o Scalar.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Aplica migrations pendentes automaticamente ao iniciar (Railway não tem step de migrate no deploy)
// Ignorado em testes de integração que usam SQLite + EnsureCreated()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetService<AppDbContext>();
    if (db?.Database.IsNpgsql() == true)
        db.Database.Migrate();
}

// Tratamento global de exceções — nunca expor stack trace em produção
app.UseExceptionHandler(exApp => exApp.Run(async ctx =>
{
    ctx.Response.StatusCode  = StatusCodes.Status500InternalServerError;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = "Ocorreu um erro interno. Tente novamente." });
}));

// Railway termina TLS no proxy — informa o scheme real ao ASP.NET Core
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Security Headers
app.UseSecurityHeaders(policies => policies
    .AddDefaultSecurityHeaders()          // X-Content-Type-Options, X-Frame-Options, Referrer-Policy
    .AddStrictTransportSecurity(maxAgeInSeconds: 60 * 60 * 24 * 365, includeSubdomains: true, preload: false)
    .AddContentSecurityPolicy(csp =>
    {
        csp.AddDefaultSrc().Self();
        csp.AddObjectSrc().None();
        csp.AddBlockAllMixedContent();
        csp.AddImgSrc().Self().Data();
        csp.AddFormAction().Self();
        csp.AddFontSrc().Self().From("https://cdn.jsdelivr.net");
        csp.AddStyleSrc().Self().UnsafeInline().From("https://cdn.jsdelivr.net");  // Bootstrap usa inline styles + Bootstrap Icons CDN
        csp.AddScriptSrc().Self().UnsafeEval().UnsafeInline();   // Blazor WASM precisa de eval + inline importmap
        csp.AddConnectSrc().Self();               // Chamadas à API
        csp.AddWorkerSrc().Self().Blob();         // Web workers do Blazor WASM
    }));

app.UseRateLimiter();

// OpenAPI + Scalar UI — não expor em Production
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

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

// ── Endpoints de infra (sem prefixo /api — usados diretamente pelo browser/OAuth) ──

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

// ── API de negócio (prefixo /api — não conflitam com rotas Blazor) ────────────

// Cálculos dinâmicos
app.MapGet("/api/calculos/pessoal", (int dia, int mes) =>
{
    if (dia < 1 || dia > 31 || mes < 1 || mes > 12)
        return Results.BadRequest(new { error = "Dia (1-31) e mês (1-12) inválidos." });

    var calc = new Numerologia.Core.Calculos.CalculosPessoais();
    var nascimento = new DateOnly(2000, mes, dia); // só dia e mês importam
    var hoje = DateOnly.FromDateTime(DateTime.Today);
    var resultado = calc.Calcular(nascimento, hoje);
    return Results.Ok(new { resultado.AnoPessoal, resultado.MesPessoal, resultado.DiaPessoal });
});

// Consulentes
app.MapGet("/api/consulentes", async (HttpContext ctx, IConsulentesRepository repo, UsuarioService usuarioService) =>
{
    var usuario = await ResolverUsuario(ctx, usuarioService);
    if (usuario is null) return Results.Unauthorized();

    var lista = await repo.ObterTodosAsync(usuario.Id);
    return Results.Ok(lista.Select(ToResponse));
}).RequireAuthorization().RequireRateLimiting("api-geral");

app.MapPost("/api/consulentes", async (CriarConsulenteRequest req, HttpContext ctx,
    IConsulentesRepository repo, UsuarioService usuarioService) =>
{
    var usuario = await ResolverUsuario(ctx, usuarioService);
    if (usuario is null) return Results.Unauthorized();

    if (!DateOnly.TryParse(req.DataNascimento, out var dataNasc))
        return Results.BadRequest(new { error = "DataNascimento inválida. Use o formato YYYY-MM-DD." });

    var consulente = new Consulente(usuario.Id, req.NomeCompleto, dataNasc, req.Email, req.Telefone);
    await repo.AdicionarAsync(consulente);

    var response = ToResponse(consulente);
    return Results.Created($"/api/consulentes/{consulente.Id}", response);
}).RequireAuthorization().RequireRateLimiting("api-geral");

app.MapGet("/api/consulentes/{id:int}", async (int id, HttpContext ctx,
    IConsulentesRepository repo, UsuarioService usuarioService) =>
{
    var usuario = await ResolverUsuario(ctx, usuarioService);
    if (usuario is null) return Results.Unauthorized();

    var consulente = await repo.ObterPorIdAsync(id, usuario.Id);
    return consulente is null ? Results.NotFound() : Results.Ok(ToResponse(consulente));
}).RequireAuthorization().RequireRateLimiting("api-geral");

app.MapPut("/api/consulentes/{id:int}", async (int id, AtualizarConsulenteRequest req, HttpContext ctx,
    IConsulentesRepository repo, UsuarioService usuarioService) =>
{
    var usuario = await ResolverUsuario(ctx, usuarioService);
    if (usuario is null) return Results.Unauthorized();

    var consulente = await repo.ObterPorIdAsync(id, usuario.Id);
    if (consulente is null) return Results.NotFound();

    if (!DateOnly.TryParse(req.DataNascimento, out var dataNasc))
        return Results.BadRequest(new { error = "DataNascimento inválida. Use o formato YYYY-MM-DD." });

    consulente.Atualizar(req.NomeCompleto, dataNasc, req.Email, req.Telefone);
    await repo.SalvarAlteracoesAsync();
    return Results.Ok(ToResponse(consulente));
}).RequireAuthorization().RequireRateLimiting("api-geral");

app.MapDelete("/api/consulentes/{id:int}", async (int id, HttpContext ctx,
    IConsulentesRepository repo, UsuarioService usuarioService) =>
{
    var usuario = await ResolverUsuario(ctx, usuarioService);
    if (usuario is null) return Results.Unauthorized();

    var consulente = await repo.ObterPorIdAsync(id, usuario.Id);
    if (consulente is null) return Results.NotFound();

    await repo.RemoverAsync(consulente);
    return Results.NoContent();
}).RequireAuthorization().RequireRateLimiting("api-geral");

// Mapas
app.MapPost("/api/consulentes/{consulenteId:int}/mapas",
    async (int consulenteId, CriarMapaRequest req, HttpContext ctx,
        IConsulentesRepository consultesRepo, IMapasRepository mapaRepo,
        GeradorMapa gerador, UsuarioService usuarioService) =>
    {
        var usuario = await ResolverUsuario(ctx, usuarioService);
        if (usuario is null) return Results.Unauthorized();

        var consulente = await consultesRepo.ObterPorIdAsync(consulenteId, usuario.Id);
        if (consulente is null) return Results.NotFound();

        var mapa = gerador.Gerar(consulente.Id, req.NomeUtilizado, consulente.DataNascimento);
        await mapaRepo.AdicionarAsync(mapa);
        await mapaRepo.SalvarAlteracoesAsync();

        return Results.Created($"/api/consulentes/{consulenteId}/mapas/{mapa.Id}",
            ToResumoResponse(mapa));
    }).RequireAuthorization().RequireRateLimiting("mapas");

app.MapGet("/api/consulentes/{consulenteId:int}/mapas",
    async (int consulenteId, HttpContext ctx,
        IMapasRepository repo, UsuarioService usuarioService) =>
    {
        var usuario = await ResolverUsuario(ctx, usuarioService);
        if (usuario is null) return Results.Unauthorized();

        var lista = await repo.ObterTodosAsync(consulenteId, usuario.Id);
        if (lista is null) return Results.NotFound();

        return Results.Ok(lista.Select(ToResumoResponse));
    }).RequireAuthorization().RequireRateLimiting("api-geral");

app.MapGet("/api/consulentes/{consulenteId:int}/mapas/{mapaId:int}",
    async (int consulenteId, int mapaId, HttpContext ctx,
        IMapasRepository repo, UsuarioService usuarioService) =>
    {
        var usuario = await ResolverUsuario(ctx, usuarioService);
        if (usuario is null) return Results.Unauthorized();

        var mapa = await repo.ObterPorIdAsync(mapaId, consulenteId, usuario.Id);
        return mapa is null ? Results.NotFound() : Results.Ok(ToDetalheResponse(mapa));
    }).RequireAuthorization().RequireRateLimiting("api-geral");

// Fallback para o roteamento client-side do Blazor
// Só alcançado quando o path NÃO começa com /api — resolve o conflito de rotas
app.MapFallbackToFile("index.html");

app.Run();

// ── Funções locais (devem vir antes das declarações de tipo) ─────────────────

static async Task<Numerologia.Core.Entities.Usuario?> ResolverUsuario(
    HttpContext ctx, UsuarioService svc)
{
    var googleId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
    return string.IsNullOrEmpty(googleId) ? null : await svc.ObterPorGoogleIdAsync(googleId);
}

static ConsulenteResponse ToResponse(Consulente c) =>
    new(c.Id, c.NomeCompleto, c.DataNascimento, c.Email, c.Telefone, c.CriadoEm);

static MapaResumoResponse ToResumoResponse(Numerologia.Core.Entities.MapaNumerologico m) =>
    new(m.Id, m.NomeUtilizado, m.DataNascimento, m.NumeroMotivacao, m.NumeroImpressao, m.NumeroExpressao, m.NumeroDestino, m.CriadoEm);

static MapaDetalheResponse ToDetalheResponse(Numerologia.Core.Entities.MapaNumerologico m) =>
    new(m.Id, m.NomeUtilizado, m.DataNascimento, m.CriadoEm,
        m.GradeLetras,
        m.NumeroMotivacao, m.NumeroImpressao, m.NumeroExpressao,
        m.DividasCarmicas, m.FiguraA,
        m.LicoesCarmicas, m.TendenciasOcultas, m.RespostaSubconsciente,
        m.MesNascimentoReduzido, m.DiaNascimentoReduzido, m.AnoNascimentoReduzido,
        m.NumeroDestino, m.Missao,
        m.CicloVida1, m.CicloVida2, m.CicloVida3,
        m.FimCiclo1Idade, m.FimCiclo2Idade,
        m.Desafio1, m.Desafio2, m.DesafioPrincipal,
        m.MomentoDecisivo1, m.MomentoDecisivo2, m.MomentoDecisivo3, m.MomentoDecisivo4,
        m.DiasMesFavoraveis, m.NumerosHarmonicos,
        m.RelacaoIntervalores,
        m.HarmoniaVibraCom, m.HarmoniaAtrai, m.HarmoniaEOpostoA,
        m.HarmoniaProfundamenteOpostoA, m.HarmoniaEPassivoEm,
        m.CoresFavoraveis);

// ── Declarações de tipo ───────────────────────────────────────────────────────

// Necessário para WebApplicationFactory nos testes de integração
public partial class Program { }

record CriarConsulenteRequest(
    [property: MaxLength(256)] string NomeCompleto,
    string DataNascimento,
    [property: MaxLength(256)] string? Email,
    [property: MaxLength(30)]  string? Telefone);

record AtualizarConsulenteRequest(
    [property: MaxLength(256)] string NomeCompleto,
    string DataNascimento,
    [property: MaxLength(256)] string? Email,
    [property: MaxLength(30)]  string? Telefone);

record ConsulenteResponse(int Id, string NomeCompleto, DateOnly DataNascimento,
    string? Email, string? Telefone, DateTime CriadoEm);

record CriarMapaRequest(string NomeUtilizado);

record MapaResumoResponse(int Id, string NomeUtilizado, DateOnly DataNascimento,
    int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao, int NumeroDestino, DateTime CriadoEm);

record MapaDetalheResponse(
    int Id, string NomeUtilizado, DateOnly DataNascimento, DateTime CriadoEm,
    Numerologia.Core.Calculos.EntradaLetra[] GradeLetras,
    int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao,
    int[] DividasCarmicas, Dictionary<int, int> FiguraA,
    int[] LicoesCarmicas, int[] TendenciasOcultas, int RespostaSubconsciente,
    int MesNascimentoReduzido, int DiaNascimentoReduzido, int AnoNascimentoReduzido,
    int NumeroDestino, int Missao,
    int CicloVida1, int CicloVida2, int CicloVida3,
    int FimCiclo1Idade, int FimCiclo2Idade,
    int Desafio1, int Desafio2, int DesafioPrincipal,
    int MomentoDecisivo1, int MomentoDecisivo2, int MomentoDecisivo3, int MomentoDecisivo4,
    int[] DiasMesFavoraveis, int[] NumerosHarmonicos,
    int RelacaoIntervalores,
    int HarmoniaVibraCom, int[] HarmoniaAtrai, int[] HarmoniaEOpostoA,
    int[] HarmoniaProfundamenteOpostoA, int[] HarmoniaEPassivoEm,
    string[] CoresFavoraveis);
