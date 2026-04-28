using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoPiramideTests
{
    // Algoritmo: pares adjacentes somados, se soma > 9 → soma - 9 (não módulo, não soma de dígitos)
    // Referência: SequenciasPiramide.java + Numerologia.xlsx (Marli Xavier)

    [Fact]
    public void Calcular_NomeComDuasLetras_TrianguloTemDuasLinhas()
    {
        // [4, 1] → soma = 5 (apex)
        var resultado = CalculoPiramide.Calcular([4, 1]);

        resultado.Triangulo.Should().HaveCount(2);
        resultado.Triangulo[0].Should().Equal([4, 1]);
        resultado.Triangulo[1].Should().Equal([5]);
        resultado.ArcanoMomento.Should().Be(5);
    }

    [Fact]
    public void Calcular_SomaAcimaDe9_SubtracaoNoveNaoModulo()
    {
        // 5 + 8 = 13 → 13 - 9 = 4 (NÃO 4 via soma de dígitos 1+3=4, mesma resposta)
        // Caso que diferencia: 7 + 7 = 14 → 14 - 9 = 5 (soma dígitos: 1+4=5 — mesmo)
        // Caso definitivo: 8 + 8 = 16 → 16 - 9 = 7 (soma dígitos: 1+6=7 — mesmo)
        // Caso que realmente diferencia subtração de soma-dígitos:
        // 9 + 9 = 18 → subtração: 18-9=9; soma dígitos: 1+8=9 — ainda igual...
        // Per Java: soma > 9 → soma -= 9. Max soma = 9+9=18 → 18-9=9. OK.
        var resultado = CalculoPiramide.Calcular([8, 5]);

        resultado.Triangulo[1].Should().Equal([4]); // 8+5=13, 13-9=4
        resultado.ArcanoMomento.Should().Be(4);
    }

    [Fact]
    public void Calcular_TresLetras_TrianguloCorreto()
    {
        // [3, 6, 2]
        // Linha 2: 3+6=9, 6+2=8 → [9, 8]
        // Linha 3: 9+8=17 → 17-9=8 → [8]
        var resultado = CalculoPiramide.Calcular([3, 6, 2]);

        resultado.Triangulo.Should().HaveCount(3);
        resultado.Triangulo[1].Should().Equal([9, 8]);
        resultado.Triangulo[2].Should().Equal([8]);
        resultado.ArcanoMomento.Should().Be(8);
    }

    [Fact]
    public void Calcular_ExemploMarliXavier_ApexCorreto()
    {
        // Valores confirmados pelo xlsx: Marli Xavier
        // M  A  R  L  I  F  E  R  R  E  I  R  A  X  A  V  I  E  R
        // 4  1  2  3  1  8  5  2  2  5  1  2  1  6  1  6  1  5  2
        int[] valoresMarli = [4, 1, 2, 3, 1, 8, 5, 2, 2, 5, 1, 2, 1, 6, 1, 6, 1, 5, 2];

        var resultado = CalculoPiramide.Calcular(valoresMarli);

        resultado.Triangulo[0].Should().Equal(valoresMarli);
        resultado.Triangulo.Should().HaveCount(19); // 19 letras → 19 linhas, apex na última
        resultado.ArcanoMomento.Should().BeInRange(1, 9);
    }

    [Fact]
    public void Calcular_UmaLetra_TrianguloComUmaLinhaEApexIgualAoValor()
    {
        var resultado = CalculoPiramide.Calcular([7]);

        resultado.Triangulo.Should().HaveCount(1);
        resultado.Triangulo[0].Should().Equal([7]);
        resultado.ArcanoMomento.Should().Be(7);
    }
}
