using FluentAssertions;
using Numerologia.Core.Services;

namespace Numerologia.UnitTests.Services;

public class InterpretacoesNumerologicasTests
{
    // ── DiaNascimento — 31 dias sem redução (pág. 135–175) ──────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(22)]
    [InlineData(28)]
    [InlineData(31)]
    public void DiaNascimento_DiasValidos_RetornaTextoNaoVazio(int dia)
    {
        InterpretacoesNumerologicas.DiaNascimento(dia)
            .Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    [InlineData(-1)]
    public void DiaNascimento_DiasInvalidos_RetornaVazio(int dia)
    {
        InterpretacoesNumerologicas.DiaNascimento(dia)
            .Should().BeEmpty();
    }

    [Fact]
    public void DiaNascimento_Dia28_ContemQuerer()
    {
        // Dia 28 = DIA DO QUERER
        InterpretacoesNumerologicas.DiaNascimento(28).ToUpperInvariant()
            .Should().Contain("QUERER");
    }

    [Fact]
    public void DiaNascimento_Dia28_ContemPositivosENegativos()
    {
        var texto = InterpretacoesNumerologicas.DiaNascimento(28);
        texto.Should().Contain("Positivos:");
        texto.Should().Contain("Negativos:");
    }

    [Fact]
    public void DiaNascimento_Dia10_ContemAutoconfianca()
    {
        // Dia 10 = DIA DA AUTOCONFIANÇA → deve conter "autoconfiança"
        InterpretacoesNumerologicas.DiaNascimento(10)
            .Should().Contain("autoconfiança");
    }

    [Fact]
    public void DiaNascimento_TodosOsDias1Ate31_RetornamTexto()
    {
        for (int dia = 1; dia <= 31; dia++)
        {
            InterpretacoesNumerologicas.DiaNascimento(dia)
                .Should().NotBeNullOrEmpty($"dia {dia} deve ter interpretação");
        }
    }
}
