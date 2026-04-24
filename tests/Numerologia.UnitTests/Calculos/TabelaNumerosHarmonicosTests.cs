using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class TabelaNumerosHarmonicosTests
{
    // Tabela pág. 219 — "Números que se Harmonizam de Acordo com o Dia do Nascimento"
    // Chave: dia de nascimento reduzido (1–9)

    public static TheoryData<int, int[]> DadosSeHarmonizamCom => new()
    {
        { 1, [2, 4, 9] },
        { 2, [1, 2, 3, 4, 5, 6, 7, 8, 9] },
        { 3, [2, 3, 6, 8, 9] },
        { 4, [1, 2, 7] },
        { 5, [2, 5, 6, 7, 9] },
        { 6, [2, 3, 4, 5, 6, 9] },
        { 7, [2, 4, 5, 7] },
        { 8, [2, 3, 9] },
        { 9, [1, 2, 3, 5, 6, 8, 9] },
    };

    [Theory]
    [MemberData(nameof(DadosSeHarmonizamCom))]
    public void Consultar_SeHarmonizamCom_RetornaListaCorreta(int diaReduzido, int[] esperados)
    {
        var r = TabelaNumerosHarmonicos.Consultar(diaReduzido);

        r.SeHarmonizamCom.Should().Equal(esperados);
    }

    public static TheoryData<int, int[]> DadosNeutroCom => new()
    {
        { 1, [1, 5, 6, 8] },
        { 2, [] },             // número 2 harmoniza com todos — sem neutros
        { 3, [4, 7] },
        { 4, [3, 5, 9] },
        { 5, [1, 4] },
        { 6, [1] },
        { 7, [3, 9] },
        { 8, [1, 6] },
        { 9, [4, 7] },
    };

    [Theory]
    [MemberData(nameof(DadosNeutroCom))]
    public void Consultar_NeutroCom_RetornaListaCorreta(int diaReduzido, int[] esperados)
    {
        var r = TabelaNumerosHarmonicos.Consultar(diaReduzido);

        r.NeutroCom.Should().Equal(esperados);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void Consultar_DiaForaDaTabela_LancaArgumentOutOfRangeException(int diaReduzido)
    {
        var act = () => TabelaNumerosHarmonicos.Consultar(diaReduzido);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
