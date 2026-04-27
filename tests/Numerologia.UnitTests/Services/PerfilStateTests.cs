using FluentAssertions;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Services;

public class PerfilStateTests
{
    [Fact]
    public void NomeExibicao_Inicial_DeveSerNulo()
    {
        var state = new PerfilState();

        state.NomeExibicao.Should().BeNull();
    }

    [Fact]
    public void Atualizar_DeveAlterarNomeExibicao()
    {
        var state = new PerfilState();

        state.Atualizar("Ana Numeróloga");

        state.NomeExibicao.Should().Be("Ana Numeróloga");
    }

    [Fact]
    public void Atualizar_DeveDispararOnChange()
    {
        var state = new PerfilState();
        var chamado = false;
        state.OnChange += () => chamado = true;

        state.Atualizar("Ana Numeróloga");

        chamado.Should().BeTrue();
    }

    [Fact]
    public void Atualizar_ComNulo_DeveLimparNomeExibicao()
    {
        var state = new PerfilState();
        state.Atualizar("Nome Antigo");

        state.Atualizar(null);

        state.NomeExibicao.Should().BeNull();
    }
}
