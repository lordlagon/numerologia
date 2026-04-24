using FluentAssertions;
using Numerologia.Core.Entities;

namespace Numerologia.UnitTests.Entities;

public class ConsulenteTests
{
    [Fact]
    public void Construtor_AtribuiPropriedadesCorretamente()
    {
        var dataNasc = new DateOnly(1990, 6, 15);

        var c = new Consulente(usuarioId: 1, nomeCompleto: "João da Silva",
            dataNascimento: dataNasc, email: "joao@email.com", telefone: "11999999999");

        c.UsuarioId.Should().Be(1);
        c.NomeCompleto.Should().Be("João da Silva");
        c.DataNascimento.Should().Be(dataNasc);
        c.Email.Should().Be("joao@email.com");
        c.Telefone.Should().Be("11999999999");
        c.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Construtor_EmailETelefoneOpcionais()
    {
        var c = new Consulente(usuarioId: 1, nomeCompleto: "Maria",
            dataNascimento: new DateOnly(1985, 3, 20));

        c.Email.Should().BeNull();
        c.Telefone.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeVazio_LancaArgumentException(string nome)
    {
        var act = () => new Consulente(usuarioId: 1, nomeCompleto: nome,
            dataNascimento: new DateOnly(1990, 1, 1));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Atualizar_AlteraPropriedadesCorretamente()
    {
        var c = new Consulente(usuarioId: 1, nomeCompleto: "João",
            dataNascimento: new DateOnly(1990, 6, 15));

        c.Atualizar("João Atualizado", new DateOnly(1991, 7, 20), "novo@email.com", "11888888888");

        c.NomeCompleto.Should().Be("João Atualizado");
        c.DataNascimento.Should().Be(new DateOnly(1991, 7, 20));
        c.Email.Should().Be("novo@email.com");
        c.Telefone.Should().Be("11888888888");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Atualizar_NomeVazio_LancaArgumentException(string nome)
    {
        var c = new Consulente(usuarioId: 1, nomeCompleto: "João",
            dataNascimento: new DateOnly(1990, 6, 15));

        var act = () => c.Atualizar(nome, new DateOnly(1990, 1, 1), null, null);

        act.Should().Throw<ArgumentException>();
    }
}
