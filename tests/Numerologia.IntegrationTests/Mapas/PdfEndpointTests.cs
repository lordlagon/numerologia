using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Mapas;

public class PdfEndpointTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId = "google-pdf-test";
    private const string Email    = "pdf@test.com";
    private const string Nome     = "Numeróloga PDF";

    private int _consulenteId;
    private const string OutroGoogleId = "google-pdf-outro";

    public PdfEndpointTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var (gid, mail, nm) in new[]
        {
            (GoogleId,    Email,              Nome),
            (OutroGoogleId, "outro-pdf@test.com", "Outro PDF"),
        })
        {
            if (!db.Usuarios.Any(u => u.GoogleId == gid))
                db.Usuarios.Add(new Usuario(gid, mail, nm));
        }
        db.SaveChanges();

        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        if (!db.Consulentes.Any(c => c.UsuarioId == usuario.Id))
        {
            db.Consulentes.Add(new Consulente(usuario.Id, "Maria Oliveira", new DateOnly(1990, 6, 15)));
            db.SaveChanges();
        }

        _consulenteId = db.Consulentes.First(c => c.UsuarioId == usuario.Id).Id;
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    // ─── GET /api/consulentes/{id}/mapas/{mapaId}/pdf ────────────────────────

    [Fact]
    public async Task GetPdf_SemAutenticacao_Retorna401()
    {
        var client = _factory.CreateUnauthenticatedClient();
        var mapaId = await CriarMapaId(AuthClient(), "MARIA OLIVEIRA");

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/pdf");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetPdf_MapaExistente_Retorna200ComPdf()
    {
        var client = AuthClient();
        var mapaId = await CriarMapaId(client, "MARIA OLIVEIRA");

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/pdf");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Should().NotBeEmpty();
        // PDF começa com %PDF
        bytes[..4].Should().BeEquivalentTo("%PDF"u8.ToArray());
    }

    [Fact]
    public async Task GetPdf_MapaExistente_RetornaHeaderComNomeArquivo()
    {
        var client = AuthClient();
        var mapaId = await CriarMapaId(client, "MARIA OLIVEIRA");

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/pdf");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentDisposition = response.Content.Headers.ContentDisposition;
        contentDisposition.Should().NotBeNull();
        contentDisposition!.FileNameStar.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetPdf_MapaInexistente_Retorna404()
    {
        var client = AuthClient();

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/99999/pdf");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPdf_ConsulenteDoutroUsuario_Retorna404()
    {
        var outroClient = _factory.CreateAuthenticatedClient(OutroGoogleId, "outro-pdf@test.com", "Outro PDF");
        var mapaId = await CriarMapaId(AuthClient(), "MARIA PDF ISO");

        var response = await outroClient.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{mapaId}/pdf");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── helper ───────────────────────────────────────────────────────────────

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
}
