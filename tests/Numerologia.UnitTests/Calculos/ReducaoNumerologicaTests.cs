using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class ReducaoNumerologicaTests
{
    // Números já simples — não mudam
    [Theory]
    [InlineData(1, 1)] [InlineData(5, 5)] [InlineData(9, 9)]
    public void Reduzir_NumeroSimples_RetornaMesmo(int numero, int esperado)
    {
        ReducaoNumerologica.Reduzir(numero).Should().Be(esperado);
    }

    // Números mestres — nunca reduzidos
    [Theory]
    [InlineData(11, 11)] [InlineData(22, 22)]
    public void Reduzir_NumeroMestre_NaoReduz(int numero, int esperado)
    {
        ReducaoNumerologica.Reduzir(numero).Should().Be(esperado);
    }

    // Redução simples (uma passagem)
    [Theory]
    [InlineData(10, 1)]  // 1+0=1
    [InlineData(19, 1)]  // 1+9=10 → 1+0=1
    [InlineData(24, 6)]  // 2+4=6
    [InlineData(38, 11)] // 3+8=11 → mestre, para
    [InlineData(29, 11)] // 2+9=11 → mestre, para
    [InlineData(33, 6)]  // 3+3=6  → 33 não é mestre neste sistema (só 11 e 22)
    public void Reduzir_NumeroComposto_ReducIterativamente(int numero, int esperado)
    {
        ReducaoNumerologica.Reduzir(numero).Should().Be(esperado);
    }

    // Redução iterativa (múltiplas passagens)
    [Theory]
    [InlineData(999, 9)] // 9+9+9=27 → 2+7=9
    [InlineData(199, 1)] // 1+9+9=19 → 1+9=10 → 1+0=1
    [InlineData(46,  1)] // 4+6=10 → 1+0=1
    public void Reduzir_NumeroGrande_ReducIterativamente(int numero, int esperado)
    {
        ReducaoNumerologica.Reduzir(numero).Should().Be(esperado);
    }

    // Casos reais do livro (pág. 96–97)
    [Theory]
    [InlineData(24, 6)]  // 11/06/1960 → soma=24 → 6
    [InlineData(31, 4)]  // 29/02/1971 → soma=31 → 4
    [InlineData(20, 2)]  // 03/01/1960 → soma=20 → 2
    public void Reduzir_ExemplosDoLivro_RetornaCorreto(int soma, int esperado)
    {
        ReducaoNumerologica.Reduzir(soma).Should().Be(esperado);
    }
}
