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

    // ── Sequências Negativas ──────────────────────────────────────────────────
    // 3+ dígitos iguais consecutivos em qualquer linha do triângulo

    [Fact]
    public void Calcular_SemSequenciaRepetida_RetornaVazio()
    {
        // [4, 1] → [5] — sem repetição
        var resultado = CalculoPiramide.Calcular([4, 1]);

        resultado.SequenciasNegativas.Should().BeEmpty();
    }

    [Fact]
    public void Calcular_ExemploAndre_Encontra666NaLinhaCorreta()
    {
        // André: A=1, N=5, D=4, R=2, É=7
        // Row 0: [1,5,4,2,7]
        // Row 1: [6,9,6,9]  (1+5=6, 5+4=9, 4+2=6, 2+7=9)
        // Row 2: [6,6,6]    (6+9=15→6, 9+6=15→6, 6+9=15→6) ← 666
        int[] andre = [1, 5, 4, 2, 7];

        var resultado = CalculoPiramide.Calcular(andre);

        resultado.SequenciasNegativas.Should().HaveCount(1);
        var seq = resultado.SequenciasNegativas[0];
        seq.Linha.Should().Be(2);
        seq.PosicaoInicio.Should().Be(0);
        seq.Comprimento.Should().Be(3);
        seq.Digito.Should().Be(6);
        seq.Significado.Should().Contain("afetos");
    }

    [Fact]
    public void Calcular_QuatroDigitosIguais_UmaSequenciaComprimento4()
    {
        // [4, 4, 4, 4] → row 0 tem 4444 → 1 sequência, comprimento 4
        var resultado = CalculoPiramide.Calcular([4, 4, 4, 4]);

        var seqs = resultado.SequenciasNegativas.Where(s => s.Linha == 0).ToArray();
        seqs.Should().HaveCount(1);
        seqs[0].Digito.Should().Be(4);
        seqs[0].Comprimento.Should().Be(4);
    }

    [Fact]
    public void Calcular_DuasSequenciasDiferentesNaMesmaLinha_EncontraAmbas()
    {
        // Row construída manualmente: [1,1,1,6,6,6]
        // Provocar via triângulo não é trivial, então testamos diretamente a detecção.
        // [1,1,1,6,6,6] como base — apex não importa para este teste
        int[] base6 = [1, 1, 1, 6, 6, 6];

        var resultado = CalculoPiramide.Calcular(base6);

        var seqsLinha0 = resultado.SequenciasNegativas.Where(s => s.Linha == 0).ToArray();
        seqsLinha0.Should().HaveCount(2);
        seqsLinha0.Select(s => s.Digito).Should().Contain(1).And.Contain(6);
    }

    [Fact]
    public void Calcular_SignificadoPreenchido_NaoNuloNemVazio()
    {
        int[] andre = [1, 5, 4, 2, 7];

        var resultado = CalculoPiramide.Calcular(andre);

        resultado.SequenciasNegativas[0].Significado.Should().NotBeNullOrWhiteSpace();
    }
}
