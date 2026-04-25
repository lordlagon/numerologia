using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class TabelaHarmoniaConjugalTests
{
    // Tabela da pág. 212 do livro — baseada no Nº de Expressão

    [Theory]
    [InlineData(1, 9)]
    [InlineData(2, 8)]
    [InlineData(3, 7)]
    [InlineData(4, 6)]
    [InlineData(5, 5)]
    [InlineData(6, 4)]
    [InlineData(7, 3)]
    [InlineData(8, 2)]
    [InlineData(9, 1)]
    public void Consultar_VibraCom_RetornaNumeroCorreto(int expressao, int esperado)
    {
        var r = TabelaHarmoniaConjugal.Consultar(expressao);

        r.VibraCom.Should().Be(esperado);
    }

    public static TheoryData<int, int[]> DadosAtrai => new()
    {
        { 1, [4, 8] },
        { 2, [7, 9] },
        { 3, [5, 6, 9] },
        { 4, [1, 8] },
        { 5, [3, 9] },
        { 6, [3, 7, 9] },
        { 7, [2, 6] },
        { 8, [1, 4] },
        { 9, [2, 3, 5, 6] }
    };

    [Theory]
    [MemberData(nameof(DadosAtrai))]
    public void Consultar_Atrai_RetornaListaCorreta(int expressao, int[] esperados)
    {
        var r = TabelaHarmoniaConjugal.Consultar(expressao);

        r.Atrai.Should().Equal(esperados);
    }

    public static TheoryData<int, int[]> DadosEOpostoA => new()
    {
        { 1, [6, 7] },
        { 2, [5] },
        { 3, [4, 8] },
        { 4, [3, 5] },
        { 5, [2, 4] },
        { 6, [1, 8] },
        { 7, [1, 9] },
        { 8, [3, 6] },
        { 9, [7] }
    };

    [Theory]
    [MemberData(nameof(DadosEOpostoA))]
    public void Consultar_EOpostoA_RetornaListaCorreta(int expressao, int[] esperados)
    {
        var r = TabelaHarmoniaConjugal.Consultar(expressao);

        r.EOpostoA.Should().Equal(esperados);
    }

    [Fact]
    public void Consultar_Numero5_ProfundamenteOpostoAo6()
    {
        var r = TabelaHarmoniaConjugal.Consultar(5);

        r.ProfundamenteOpostoA.Should().Equal([6]);
    }

    [Fact]
    public void Consultar_Numero6_ProfundamenteOpostoAo5()
    {
        var r = TabelaHarmoniaConjugal.Consultar(6);

        r.ProfundamenteOpostoA.Should().Equal([5]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void Consultar_OutrosNumeros_ProfundamenteOpostoAVazio(int expressao)
    {
        var r = TabelaHarmoniaConjugal.Consultar(expressao);

        r.ProfundamenteOpostoA.Should().BeEmpty();
    }

    public static TheoryData<int, int[]> DadosEPassivoEm => new()
    {
        { 1, [2, 3, 5] },
        { 2, [1, 3, 4, 6] },
        { 3, [1, 2] },
        { 4, [2, 7, 9] },
        { 5, [1, 7, 8] },
        { 6, [2] },
        { 7, [4, 5, 8] },
        { 8, [5, 7, 9] },
        { 9, [4, 8] }
    };

    [Theory]
    [MemberData(nameof(DadosEPassivoEm))]
    public void Consultar_EPassivoEm_RetornaListaCorreta(int expressao, int[] esperados)
    {
        var r = TabelaHarmoniaConjugal.Consultar(expressao);

        r.EPassivoEm.Should().Equal(esperados);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void Consultar_NumeroForaDaTabela_LancaArgumentOutOfRangeException(int expressao)
    {
        var act = () => TabelaHarmoniaConjugal.Consultar(expressao);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
