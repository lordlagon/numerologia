using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Numerologia.Infrastructure.Data;
using Numerologia.IntegrationTests.Infraestrutura;

namespace Numerologia.IntegrationTests.Auth;

public class AuthEndpointsTests : IClassFixture<NumerologiaWebFactory>
{
    private readonly NumerologiaWebFactory _factory;

    public AuthEndpointsTests(NumerologiaWebFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthLogin_DeveRedirecionarParaGoogle()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/auth/login");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("google");
    }

    [Fact]
    public async Task AuthMe_SemAutenticacao_DeveRedirecionarParaLogin()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.GetAsync("/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task AuthMe_ComAutenticacao_DeveRetornarDadosDoUsuario()
    {
        var client = _factory.CreateAuthenticatedClient("google-789", "teste@gmail.com", "Teste User");

        var response = await client.GetAsync("/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<MeResponse>();
        body!.Email.Should().Be("teste@gmail.com");
        body.Nome.Should().Be("Teste User");
    }

    [Fact]
    public async Task AuthMe_ComAutenticacao_DeveCriarUsuarioNoBanco()
    {
        var client = _factory.CreateAuthenticatedClient("google-novo", "novo@gmail.com", "Novo User");

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
        var client = _factory.CreateAuthenticatedClient("google-logout", "logout@test.com", "Logout");

        var response = await client.PostAsync("/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public record MeResponse(string Email, string Nome);
}
