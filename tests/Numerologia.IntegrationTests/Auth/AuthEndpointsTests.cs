using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Numerologia.Infrastructure.Data;

namespace Numerologia.IntegrationTests.Auth;

public class AuthEndpointsTests : IClassFixture<AuthEndpointsTests.NumerologiaWebFactory>, IDisposable
{
    private readonly NumerologiaWebFactory _factory;

    public AuthEndpointsTests(NumerologiaWebFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private HttpClient CreateAuthenticatedClient(string googleId, string email, string nome)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-GoogleId", googleId);
        client.DefaultRequestHeaders.Add("X-Test-Email", email);
        client.DefaultRequestHeaders.Add("X-Test-Nome", nome);
        return client;
    }

    [Fact]
    public async Task AuthLogin_DeveRedirecionarParaGoogle()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("google");
    }

    [Fact]
    public async Task AuthMe_SemAutenticacao_DeveRedirecionarParaLogin()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/auth/me");

        // Cookie auth redireciona para o challenge (Google) quando não autenticado
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task AuthMe_ComAutenticacao_DeveRetornarDadosDoUsuario()
    {
        var client = CreateAuthenticatedClient("google-789", "teste@gmail.com", "Teste User");

        var response = await client.GetAsync("/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<MeResponse>();
        body!.Email.Should().Be("teste@gmail.com");
        body.Nome.Should().Be("Teste User");
    }

    [Fact]
    public async Task AuthMe_ComAutenticacao_DeveCriarUsuarioNoBanco()
    {
        var client = CreateAuthenticatedClient("google-novo", "novo@gmail.com", "Novo User");

        await client.GetAsync("/auth/me");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.GoogleId == "google-novo");
        usuario.Should().NotBeNull();
        usuario!.Email.Should().Be("novo@gmail.com");
    }

    [Fact]
    public async Task AuthLogout_DeveRetornar200()
    {
        var client = CreateAuthenticatedClient("google-logout", "logout@test.com", "Logout");

        var response = await client.PostAsync("/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
    }

    public record MeResponse(string Email, string Nome);

    public class NumerologiaWebFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public NumerologiaWebFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Google:ClientId"] = "fake-client-id",
                    ["Google:ClientSecret"] = "fake-client-secret",
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Remove o DbContext do PostgreSQL e substitui por SQLite in-memory
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(_connection));

                services.AddDataProtection()
                    .SetApplicationName("NumerologiaTests")
                    .UseEphemeralDataProtectionProvider();

                // Adiciona FakeScheme que autentica via headers de teste
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "FakeScheme";
                    options.DefaultScheme = "FakeScheme";
                    options.DefaultChallengeScheme = "Google";
                })
                .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
                    "FakeScheme", _ => { });

                // Garante que o banco é criado
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _connection.Dispose();
        }
    }

    /// <summary>
    /// Handler que autentica quando os headers X-Test-* estão presentes.
    /// Sem headers, retorna NoResult (fallback para os schemes reais).
    /// </summary>
    public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public FakeAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-GoogleId", out var googleId) ||
                string.IsNullOrEmpty(googleId))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var email = Request.Headers["X-Test-Email"].ToString();
            var nome = Request.Headers["X-Test-Nome"].ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, googleId!),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, nome),
            };
            var identity = new ClaimsIdentity(claims, "FakeScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "FakeScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
