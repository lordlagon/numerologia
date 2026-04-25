using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Core.Services;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Dashboard;

public class DashboardEndpointTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId = "google-dashboard-test";
    private const string Email    = "dashboard@test.com";
    private const string Nome     = "Numeróloga Dashboard";

    public DashboardEndpointTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.Usuarios.Any(u => u.GoogleId == GoogleId))
        {
            db.Usuarios.Add(new Usuario(GoogleId, Email, Nome));
            db.SaveChanges();
        }
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    [Fact]
    public async Task Get_SemAutenticacao_Retorna401()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Get_UsuarioSemDados_RetornaTotaisZerados()
    {
        // Usuário isolado, nunca usado em outros testes desta classe
        const string googleIdVazio = "google-dashboard-vazio";
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!db.Usuarios.Any(u => u.GoogleId == googleIdVazio))
        {
            db.Usuarios.Add(new Usuario(googleIdVazio, "vazio@dashboard.com", "Vazio"));
            db.SaveChanges();
        }

        var client = _factory.CreateAuthenticatedClient(googleIdVazio, "vazio@dashboard.com", "Vazio");
        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();
        body.Should().NotBeNull();
        body!.TotalConsulentes.Should().Be(0);
        body.UltimosMapas.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_ComConsulentes_RetornaTotalCorreto()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        var consulente = new Consulente(usuario.Id, "Ana Dashboard", new DateOnly(1990, 5, 20));
        db.Consulentes.Add(consulente);
        db.SaveChanges();

        var client = AuthClient();
        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();
        body!.TotalConsulentes.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Get_ComMapas_RetornaUltimosMapas()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        var consulente = new Consulente(usuario.Id, "Carlos Dashboard", new DateOnly(1985, 3, 10));
        db.Consulentes.Add(consulente);
        db.SaveChanges();

        var gerador = scope.ServiceProvider.GetRequiredService<GeradorMapa>();
        var mapa = gerador.Gerar(consulente.Id, "CARLOS", new DateOnly(1985, 3, 10));
        db.Mapas.Add(mapa);
        db.SaveChanges();

        var client = AuthClient();
        var response = await client.GetAsync("/api/dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();
        body!.UltimosMapas.Should().NotBeEmpty();
        body.UltimosMapas.First().NomeConsulente.Should().NotBeNullOrWhiteSpace();
        body.UltimosMapas.First().NomeUtilizado.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Get_ComMaisDecincoMapas_RetornaNoMaximoCinco()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        var consulente = new Consulente(usuario.Id, "Maria Dashboard", new DateOnly(1992, 7, 15));
        db.Consulentes.Add(consulente);
        db.SaveChanges();

        var gerador = scope.ServiceProvider.GetRequiredService<GeradorMapa>();
        for (var i = 0; i < 7; i++)
        {
            db.Mapas.Add(gerador.Gerar(consulente.Id, "MARIA", new DateOnly(1992, 7, 15)));
        }
        db.SaveChanges();

        var client = AuthClient();
        var response = await client.GetAsync("/api/dashboard");

        var body = await response.Content.ReadFromJsonAsync<DashboardResponse>();
        body!.UltimosMapas.Should().HaveCountLessOrEqualTo(5);
    }

    private record DashboardResponse(
        int TotalConsulentes,
        List<UltimoMapaItem> UltimosMapas);

    private record UltimoMapaItem(
        int MapaId,
        int ConsulenteId,
        string NomeConsulente,
        string NomeUtilizado,
        DateTime CriadoEm);
}
