using FluentAssertions;
using Numerologia.Core.Entities;

namespace Numerologia.UnitTests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Criar_ComDadosValidos_DevePreencherPropriedades()
    {
        var usuario = new Usuario(
            googleId: "google-123",
            email: "joao@gmail.com",
            nome: "João Silva");

        usuario.GoogleId.Should().Be("google-123");
        usuario.Email.Should().Be("joao@gmail.com");
        usuario.Nome.Should().Be("João Silva");
        usuario.Id.Should().Be(0);
        usuario.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComGoogleIdInvalido_DeveLancarExcecao(string? googleId)
    {
        var act = () => new Usuario(googleId!, "email@test.com", "Nome");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("googleId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComEmailInvalido_DeveLancarExcecao(string? email)
    {
        var act = () => new Usuario("google-123", email!, "Nome");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComNomeInvalido_DeveLancarExcecao(string? nome)
    {
        var act = () => new Usuario("google-123", "email@test.com", nome!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("nome");
    }
}
