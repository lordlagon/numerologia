using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Mapas;

public class PiramidesEndpointTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId     = "google-piramides-test";
    private const string Email        = "piramides@test.com";
    private const string Nome         = "Numeróloga Pirâmides";
    private const string OutroGoogleId = "google-piramides-outro";

    private int _consulenteId;

    public PiramidesEndpointTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var (gid, mail, nm) in new[]
        {
            (GoogleId,     Email,                   Nome),
            (OutroGoogleId, "outro-pir@test.com", "Outro Pir"),
        })
        {
            if (!db.Usuarios.Any(u => u.GoogleId == gid))
                db.Usuarios.Add(new Usuario(gid, mail, nm));
        }
        db.SaveChanges();

        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        if (!db.Consulentes.Any(c => c.UsuarioId == usuario.Id))
        {
            db.Consulentes.Add(new Consulente(usuario.Id, "Ana Lima", new DateOnly(1985, 3, 22)));
            db.SaveChanges();
        }

        _consulenteId = db.Consulentes.First(c => c.UsuarioId == usuario.Id).Id;
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    // ─── GET /api/consulentes/{id}/mapas/{mapaId}/piramides ─────────────────

    [Fact]
    public async Task GetPiramides_SemAutenticacao_Retorna401()
    {
        var client  = _factory.CreateUnauthenticatedClient();
        var mapaId  = await CriarMapaId(AuthClient(), "ANA LIMA");

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/piramides");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetPiramides_MapaExistente_Retorna200ComTriangulo()
    {
        var client = AuthClient();
        var mapaId = await CriarMapaId(client, "ANA LIMA");

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/piramides");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PiramideDto>();
        body.Should().NotBeNull();
        body!.Triangulo.Should().NotBeEmpty();
        body.ArcanoMomento.Should().BeInRange(1, 9);
        // apex é a última linha com 1 elemento
        body.Triangulo[^1].Should().HaveCount(1);
        body.Triangulo[^1][0].Should().Be(body.ArcanoMomento);
        // arcanos: N letras → N-1 arcanos
        body.Arcanos.Should().HaveCount(body.Triangulo[0].Length - 1);
    }

    [Fact]
    public async Task GetPiramides_MapaInexistente_Retorna404()
    {
        var client = AuthClient();

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/99999/piramides");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPiramides_ConsulenteDoutroUsuario_Retorna404()
    {
        var outroClient = _factory.CreateAuthenticatedClient(OutroGoogleId, "outro-pir@test.com", "Outro Pir");
        var mapaId = await CriarMapaId(AuthClient(), "ANA LIMA XPTO");

        var response = await outroClient.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/piramides");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private async Task<int> CriarMapaId(HttpClient client, string nomeUtilizado)
    {
        var r = await client.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = nomeUtilizado });
        r.EnsureSuccessStatusCode();
        var body = await r.Content.ReadFromJsonAsync<MapaResumoDto>();
        return body!.Id;
    }

    private record MapaResumoDto(int Id, string NomeUtilizado, DateOnly DataNascimento,
        int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao, int NumeroDestino, DateTime CriadoEm);

    private record SequenciaNegativaDto(int Linha, int PosicaoInicio, int Comprimento, int Digito, string Significado);
    private record PiramideDto(int[][] Triangulo, int ArcanoMomento, int[] Arcanos, SequenciaNegativaDto[] SequenciasNegativas);
}
