using FluentAssertions;
using NSubstitute;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Core.Services;

namespace Numerologia.UnitTests.Services;

public class UsuarioServiceTests
{
    private readonly IUsuarioRepository _repository;
    private readonly UsuarioService _sut;

    public UsuarioServiceTests()
    {
        _repository = Substitute.For<IUsuarioRepository>();
        _sut = new UsuarioService(_repository);
    }

    [Fact]
    public async Task ObterOuCriar_QuandoUsuarioExiste_DeveRetornarExistente()
    {
        var existente = new Usuario("google-123", "joao@gmail.com", "João");
        _repository.ObterPorGoogleIdAsync("google-123").Returns(existente);

        var resultado = await _sut.ObterOuCriarAsync("google-123", "joao@gmail.com", "João");

        resultado.Should().BeSameAs(existente);
        await _repository.DidNotReceive().AdicionarAsync(Arg.Any<Usuario>());
    }

    [Fact]
    public async Task ObterOuCriar_QuandoUsuarioNaoExiste_DeveCriarERetornar()
    {
        _repository.ObterPorGoogleIdAsync("google-456").Returns((Usuario?)null);

        var resultado = await _sut.ObterOuCriarAsync("google-456", "maria@gmail.com", "Maria");

        resultado.GoogleId.Should().Be("google-456");
        resultado.Email.Should().Be("maria@gmail.com");
        resultado.Nome.Should().Be("Maria");
        await _repository.Received(1).AdicionarAsync(Arg.Is<Usuario>(u =>
            u.GoogleId == "google-456" &&
            u.Email == "maria@gmail.com" &&
            u.Nome == "Maria"));
    }
}
