using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Core.Entities;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Perfil;

public class PerfilEndpointsTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;

    private const string GoogleIdGet     = "google-perfil-get";
    private const string GoogleIdPut     = "google-perfil-put";
    private const string GoogleIdPersist = "google-perfil-persist";
    private const string GoogleIdNull    = "google-perfil-null";
    private const string GoogleIdEmpty   = "google-perfil-empty";

    public PerfilEndpointsTests(NumerologiaWebFactory factory)
    {
        _factory = factory;

        // Pré-cria todos os usuários necessários nos testes
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var googleIds = new[]
        {
            (GoogleIdGet,     "get@test.com",     "Get User"),
            (GoogleIdPut,     "put@test.com",     "Put User"),
            (GoogleIdPersist, "persist@test.com", "Persist User"),
            (GoogleIdNull,    "null@test.com",    "Null User"),
            (GoogleIdEmpty,   "empty@test.com",   "Empty User"),
        };
        foreach (var (id, email, nome) in googleIds)
        {
            if (!db.Usuarios.Any(u => u.GoogleId == id))
            {
                db.Usuarios.Add(new Usuario(id, email, nome));
            }
        }
        db.SaveChanges();
    }

    [Fact]
    public async Task GetPerfil_SemAutenticacao_DeveRetornar401OuRedirect()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/api/perfil");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetPerfil_ComAutenticacao_DeveRetornarDadosDoUsuario()
    {
        var client = _factory.CreateAuthenticatedClient(GoogleIdGet, "get@test.com", "Get User");

        var response = await client.GetAsync("/api/perfil");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PerfilResponse>();
        body!.Email.Should().Be("get@test.com");
        body.Nome.Should().Be("Get User");
        body.NomeExibicao.Should().BeNull();
    }

    [Fact]
    public async Task PutPerfil_ComNomeExibicaoValido_DeveAtualizarERetornar200()
    {
        var client = _factory.CreateAuthenticatedClient(GoogleIdPut, "put@test.com", "Put User");

        var response = await client.PutAsJsonAsync("/api/perfil",
            new AtualizarPerfilRequest("Numeróloga Ana"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PerfilResponse>();
        body!.NomeExibicao.Should().Be("Numeróloga Ana");
    }

    [Fact]
    public async Task PutPerfil_DevePeristirNomeExibicaoNoBanco()
    {
        var client = _factory.CreateAuthenticatedClient(GoogleIdPersist, "persist@test.com", "Persist User");

        await client.PutAsJsonAsync("/api/perfil",
            new AtualizarPerfilRequest("Nome Persistido"));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usuario = await db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.GoogleId == GoogleIdPersist);
        usuario!.NomeExibicao.Should().Be("Nome Persistido");
    }

    [Fact]
    public async Task PutPerfil_ComNomeExibicaoNulo_DeveLimparERetornar200()
    {
        var client = _factory.CreateAuthenticatedClient(GoogleIdNull, "null@test.com", "Null User");
        await client.PutAsJsonAsync("/api/perfil", new AtualizarPerfilRequest("Nome Antigo"));

        var response = await client.PutAsJsonAsync("/api/perfil",
            new AtualizarPerfilRequest(null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PerfilResponse>();
        body!.NomeExibicao.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task PutPerfil_ComNomeExibicaoVazio_DeveRetornar400(string nomeExibicao)
    {
        var client = _factory.CreateAuthenticatedClient(GoogleIdEmpty, "empty@test.com", "Empty User");

        var response = await client.PutAsJsonAsync("/api/perfil",
            new AtualizarPerfilRequest(nomeExibicao));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    record PerfilResponse(string Email, string Nome, string? NomeExibicao);
    record AtualizarPerfilRequest(string? NomeExibicao);
}
