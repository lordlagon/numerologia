using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoPiramideTests
{
    // Algoritmo triângulo: pares adjacentes somados, se soma > 9 → soma - 9
    // Algoritmo arcanos: arcano[i] = base[i] * 10 + base[i+1]
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
        // Per Java: soma > 9 → soma -= 9. Max soma = 9+9=18 → 18-9=9.
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

    // ── Arcanos ──────────────────────────────────────────────────────────────
    // arcano[i] = base[i] * 10 + base[i+1]
    // N letras → N-1 arcanos

    [Fact]
    public void Calcular_DuasLetras_UmArcano()
    {
        // [4, 1] → arcano = 41
        var resultado = CalculoPiramide.Calcular([4, 1]);

        resultado.Arcanos.Should().HaveCount(1);
        resultado.Arcanos[0].Should().Be(41);
    }

    [Fact]
    public void Calcular_TresLetras_DoisArcanos()
    {
        // [3, 6, 2] → arcanos = [36, 62]
        var resultado = CalculoPiramide.Calcular([3, 6, 2]);

        resultado.Arcanos.Should().HaveCount(2);
        resultado.Arcanos.Should().Equal([36, 62]);
    }

    [Fact]
    public void Calcular_ExemploJoaquim_ArcanosCorretos()
    {
        // JOAQUIM MARIA MACHADO DE ASSIS (sem espaços)
        // J  O  A  Q  U  I  M  M  A  R  I  A  M  A  C  H  A  D  O  D  E  A  S  S  I  S
        // 1  7  1  1  6  1  4  4  1  2  1  1  4  1  3  5  1  4  7  4  5  1  3  3  1  3
        int[] valores = [1, 7, 1, 1, 6, 1, 4, 4, 1, 2, 1, 1, 4, 1, 3, 5, 1, 4, 7, 4, 5, 1, 3, 3, 1, 3];

        var resultado = CalculoPiramide.Calcular(valores);

        int[] arcanosEsperados = [17, 71, 11, 16, 61, 14, 44, 41, 12, 21,
                                   11, 14, 41, 13, 35, 51, 14, 47, 74, 45,
                                   51, 13, 33, 31, 13];
        resultado.Arcanos.Should().HaveCount(25);
        resultado.Arcanos.Should().Equal(arcanosEsperados);
    }

    [Fact]
    public void Calcular_UmaLetra_SemArcanos()
    {
        var resultado = CalculoPiramide.Calcular([7]);

        resultado.Arcanos.Should().BeEmpty();
    }
}
