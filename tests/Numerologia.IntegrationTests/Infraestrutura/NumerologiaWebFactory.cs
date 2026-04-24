using System.Security.Claims;
using System.Text.Encodings.Web;
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

namespace Numerologia.IntegrationTests.Infraestrutura;

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
                ["Google:ClientId"]     = "fake-client-id",
                ["Google:ClientSecret"] = "fake-client-secret",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddDataProtection()
                .SetApplicationName("NumerologiaTests")
                .UseEphemeralDataProtectionProvider();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "FakeScheme";
                options.DefaultScheme             = "FakeScheme";
                options.DefaultChallengeScheme    = "Google";
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("FakeScheme", _ => { });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    public HttpClient CreateUnauthenticatedClient() =>
        CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public HttpClient CreateAuthenticatedClient(string googleId, string email, string nome)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-GoogleId", googleId);
        client.DefaultRequestHeaders.Add("X-Test-Email",    email);
        client.DefaultRequestHeaders.Add("X-Test-Nome",     nome);
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}

public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-GoogleId", out var googleId) ||
            string.IsNullOrEmpty(googleId))
            return Task.FromResult(AuthenticateResult.NoResult());

        var email = Request.Headers["X-Test-Email"].ToString();
        var nome  = Request.Headers["X-Test-Nome"].ToString();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, googleId!),
            new Claim(ClaimTypes.Email,          email),
            new Claim(ClaimTypes.Name,           nome),
        };
        var identity  = new ClaimsIdentity(claims, "FakeScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket    = new AuthenticationTicket(principal, "FakeScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
