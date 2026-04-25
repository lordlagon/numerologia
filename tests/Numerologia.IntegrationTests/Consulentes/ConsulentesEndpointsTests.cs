using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Consulentes;

public class ConsulentesEndpointsTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;
    private const string GoogleId = "google-consulentes-test";
    private const string Email    = "consulentes@test.com";
    private const string Nome     = "Numeróloga Teste";

    public ConsulentesEndpointsTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        // Garante que o usuário existe no banco antes dos testes
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!db.Usuarios.Any(u => u.GoogleId == GoogleId))
        {
            db.Usuarios.Add(new Usuario(GoogleId, Email, Nome));
            db.SaveChanges();
        }
    }

    private HttpClient AuthClient() => _factory.CreateAuthenticatedClient(GoogleId, Email, Nome);

    // ─── POST /consulentes ───────────────────────────────────────────────────

    [Fact]
    public async Task Post_SemAutenticacao_DeveRetornar401OuRedirect()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.PostAsJsonAsync("/api/consulentes",
            new { NomeCompleto = "Teste", DataNascimento = "1990-01-01" });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornar201ComConsulente()
    {
        var response = await AuthClient().PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto    = "Maria Teste Post",
            DataNascimento  = "1990-06-15",
            Email           = "maria@test.com",
            Telefone        = "11999999999"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<ConsulenteResponse>();
        body!.NomeCompleto.Should().Be("Maria Teste Post");
        body.Email.Should().Be("maria@test.com");
        body.Id.Should().BeGreaterThan(0);
        response.Headers.Location.Should().NotBeNull();
    }

    // ─── GET /consulentes ────────────────────────────────────────────────────

    [Fact]
    public async Task Get_DeveRetornarConsulentesDoUsuario()
    {
        await AuthClient().PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto   = "Consulente Get List",
            DataNascimento = "1985-03-20"
        });

        var response = await AuthClient().GetAsync("/api/consulentes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<ConsulenteResponse>>();
        lista.Should().NotBeEmpty();
        lista.Should().Contain(c => c.NomeCompleto == "Consulente Get List");
    }

    // ─── GET /consulentes/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetById_QuandoExiste_DeveRetornar200()
    {
        var criado = await (await AuthClient().PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto = "Consulente GetById", DataNascimento = "1980-01-01"
        })).Content.ReadFromJsonAsync<ConsulenteResponse>();

        var response = await AuthClient().GetAsync($"/api/consulentes/{criado!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ConsulenteResponse>();
        body!.NomeCompleto.Should().Be("Consulente GetById");
    }

    [Fact]
    public async Task GetById_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await AuthClient().GetAsync("/api/consulentes/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── PUT /consulentes/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task Put_QuandoExiste_DeveRetornar200EAtualizarDados()
    {
        var criado = await (await AuthClient().PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto = "Consulente Put Original", DataNascimento = "1975-07-04"
        })).Content.ReadFromJsonAsync<ConsulenteResponse>();

        var response = await AuthClient().PutAsJsonAsync($"/api/consulentes/{criado!.Id}", new
        {
            NomeCompleto   = "Consulente Put Atualizado",
            DataNascimento = "1975-07-04",
            Email          = "novo@email.com",
            Telefone       = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ConsulenteResponse>();
        body!.NomeCompleto.Should().Be("Consulente Put Atualizado");
        body.Email.Should().Be("novo@email.com");
    }

    [Fact]
    public async Task Put_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await AuthClient().PutAsJsonAsync("/api/consulentes/999999", new
        {
            NomeCompleto   = "X",
            DataNascimento = "1990-01-01",
            Email          = (string?)null,
            Telefone       = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── DELETE /consulentes/{id} ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_QuandoExiste_DeveRetornar204()
    {
        var criado = await (await AuthClient().PostAsJsonAsync("/api/consulentes", new
        {
            NomeCompleto = "Consulente Delete", DataNascimento = "1993-12-25"
        })).Content.ReadFromJsonAsync<ConsulenteResponse>();

        var response = await AuthClient().DeleteAsync($"/api/consulentes/{criado!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_QuandoNaoExiste_DeveRetornar404()
    {
        var response = await AuthClient().DeleteAsync("/api/consulentes/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ─── DTO de resposta (espelha o que a API devolve) ───────────────────────
    private record ConsulenteResponse(int Id, string NomeCompleto, DateOnly DataNascimento,
        string? Email, string? Telefone, DateTime CriadoEm);
}
