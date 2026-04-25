using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Mapas;

public class MapasEndpointsTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId = "google-mapas-test";
    private const string Email    = "mapas@test.com";
    private const string Nome     = "Numeróloga Mapas";

    private int _consulenteId;

    // Outros usuários usados em testes de isolamento multi-tenant
    private const string OutroGoogleIdPost = "outro-google-id";
    private const string OutroGoogleIdLista = "outro-google-lista";

    public MapasEndpointsTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.Usuarios.Any(u => u.GoogleId == GoogleId))
        {
            db.Usuarios.Add(new Usuario(GoogleId, Email, Nome));
            db.SaveChanges();
        }

        // Cria outros usuários para testes de isolamento
        foreach (var (gid, mail, nm) in new[]
        {
            (OutroGoogleIdPost,  "outro@test.com",       "Outro Post"),
            (OutroGoogleIdLista, "outro-lista@test.com", "Outro Lista"),
        })
        {
            if (!db.Usuarios.Any(u => u.GoogleId == gid))
            {
                db.Usuarios.Add(new Usuario(gid, mail, nm));
            }
        }
        db.SaveChanges();

        var usuario = db.Usuarios.First(u => u.GoogleId == GoogleId);

        if (!db.Consulentes.Any(c => c.UsuarioId == usuario.Id))
        {
            var consulente = new Consulente(usuario.Id, "José da Silva", new DateOnly(1985, 3, 10));
            db.Consulentes.Add(consulente);
            db.SaveChanges();
        }

        _consulenteId = db.Consulentes.First(c => c.UsuarioId == usuario.Id).Id;
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    // ─── POST /consulentes/{id}/mapas ────────────────────────────────────────

    [Fact]
    public async Task Post_SemAutenticacao_Retorna401()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = "JOSE DA SILVA" });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201ComMapa()
    {
        var client = AuthClient();

        var response = await client.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = "JOSE DA SILVA" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<MapaResumoResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.NomeUtilizado.Should().Be("JOSE DA SILVA");
        body.NumeroExpressao.Should().BeInRange(1, 22);
    }

    [Fact]
    public async Task Post_ConsulenteDoutroUsuario_Retorna404()
    {
        var outroClient = _factory.CreateAuthenticatedClient(OutroGoogleIdPost, "outro@test.com", "Outro Post");

        var response = await outroClient.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = "JOSE DA SILVA" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── GET /consulentes/{id}/mapas ─────────────────────────────────────────

    [Fact]
    public async Task GetLista_ComMapasExistentes_RetornaLista()
    {
        var client = AuthClient();
        // Garante que há pelo menos um mapa
        await client.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = "JOSE LISTA" });

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<MapaResumoResponse>>();
        body.Should().NotBeNull();
        body!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetLista_ConsulenteDoutroUsuario_Retorna404()
    {
        var outroClient = _factory.CreateAuthenticatedClient(OutroGoogleIdLista, "outro-lista@test.com", "Outro Lista");

        var response = await outroClient.GetAsync($"/api/consulentes/{_consulenteId}/mapas");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── GET /consulentes/{id}/mapas/{mapaId} ────────────────────────────────

    [Fact]
    public async Task GetDetalhe_MapaExistente_RetornaMapaCompleto()
    {
        var client = AuthClient();
        var criarResponse = await client.PostAsJsonAsync($"/api/consulentes/{_consulenteId}/mapas",
            new { NomeUtilizado = "JOSE DETALHE" });
        var criado = await criarResponse.Content.ReadFromJsonAsync<MapaResumoResponse>();

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/{criado!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<MapaDetalheResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(criado.Id);
        body.NomeUtilizado.Should().Be("JOSE DETALHE");
        body.NumeroMotivacao.Should().BeInRange(1, 22);
        body.NumeroImpressao.Should().BeInRange(1, 22);
        body.NumeroExpressao.Should().BeInRange(1, 22);
        body.NumeroDestino.Should().BeInRange(1, 22);
        body.CoresFavoraveis.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetDetalhe_MapaInexistente_Retorna404()
    {
        var client = AuthClient();

        var response = await client.GetAsync($"/api/consulentes/{_consulenteId}/mapas/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Records de resposta (espelham os do Program.cs)
    private record MapaResumoResponse(int Id, string NomeUtilizado, DateOnly DataNascimento,
        int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao, int NumeroDestino, DateTime CriadoEm);

    private record MapaDetalheResponse(int Id, string NomeUtilizado, DateOnly DataNascimento,
        DateTime CriadoEm,
        int NumeroMotivacao, int NumeroImpressao, int NumeroExpressao,
        int[] DividasCarmicas, Dictionary<string,int> FiguraA,
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
}
