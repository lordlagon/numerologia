using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoMapaTests
{
    private readonly CalculoMapa _sut = new();

    // ── MARIA ──────────────────────────────────────────────────────────────
    // M(4) A(1) R(2) I(1) A(1)
    // Vogais:     A(1) I(1) A(1) → soma=3  → Motivação=3
    // Consoantes: M(4) R(2)      → soma=6  → Impressão=6
    // Expressão:  4+1+2+1+1=9               → Expressão=9

    [Fact]
    public void Calcular_Maria_Motivacao()
    {
        var resultado = _sut.Calcular("MARIA");
        resultado.NumeroMotivacao.Should().Be(3);
    }

    [Fact]
    public void Calcular_Maria_Impressao()
    {
        var resultado = _sut.Calcular("MARIA");
        resultado.NumeroImpressao.Should().Be(6);
    }

    [Fact]
    public void Calcular_Maria_Expressao()
    {
        var resultado = _sut.Calcular("MARIA");
        resultado.NumeroExpressao.Should().Be(9);
    }

    // ── CARLOS ─────────────────────────────────────────────────────────────
    // C(3) A(1) R(2) L(3) O(7) S(3)
    // Vogais:     A(1) O(7)          → soma=8   → Motivação=8
    // Consoantes: C(3) R(2) L(3) S(3)→ soma=11  → Impressão=11 (mestre)
    // Expressão:  3+1+2+3+7+3=19 → Dívida Cármica 19 → 1+9=10 → 1+0=1

    [Fact]
    public void Calcular_Carlos_Motivacao()
    {
        var resultado = _sut.Calcular("CARLOS");
        resultado.NumeroMotivacao.Should().Be(8);
    }

    [Fact]
    public void Calcular_Carlos_Impressao_MestreOnze()
    {
        var resultado = _sut.Calcular("CARLOS");
        resultado.NumeroImpressao.Should().Be(11);
    }

    [Fact]
    public void Calcular_Carlos_Expressao_ComDividaCármica()
    {
        var resultado = _sut.Calcular("CARLOS");
        resultado.NumeroExpressao.Should().Be(1);
        resultado.DividasCarmicas.Should().Contain(19);
    }

    // ── ANDRE ──────────────────────────────────────────────────────────────
    // A(1) N(5) D(4) R(2) E(5)
    // Vogais:     A(1) E(5)     → soma=6   → Motivação=6
    // Consoantes: N(5) D(4) R(2)→ soma=11  → Impressão=11 (mestre)
    // Expressão:  1+5+4+2+5=17  → 1+7=8

    [Fact]
    public void Calcular_Andre_Motivacao()
    {
        var resultado = _sut.Calcular("ANDRE");
        resultado.NumeroMotivacao.Should().Be(6);
    }

    [Fact]
    public void Calcular_Andre_Impressao_MestreOnze()
    {
        var resultado = _sut.Calcular("ANDRE");
        resultado.NumeroImpressao.Should().Be(11);
    }

    [Fact]
    public void Calcular_Andre_Expressao()
    {
        var resultado = _sut.Calcular("ANDRE");
        resultado.NumeroExpressao.Should().Be(8);
    }

    // ── ANDRÉ (com acento agudo) ────────────────────────────────────────────
    // A(1) N(5) D(4) R(2) É(7)   ← é = e(5)+2 = 7
    // Vogais:     A(1) É(7)      → soma=8   → Motivação=8
    // Consoantes: N(5) D(4) R(2) → soma=11  → Impressão=11 (mestre)
    // Expressão:  1+5+4+2+7=19   → Dívida Cármica 19 → 1+9=10 → 1+0=1

    [Fact]
    public void Calcular_AndreComAcento_Motivacao()
    {
        var resultado = _sut.Calcular("ANDRÉ");
        resultado.NumeroMotivacao.Should().Be(8);
    }

    [Fact]
    public void Calcular_AndreComAcento_Impressao_MestreOnze()
    {
        var resultado = _sut.Calcular("ANDRÉ");
        resultado.NumeroImpressao.Should().Be(11);
    }

    [Fact]
    public void Calcular_AndreComAcento_Expressao_ComDividaCármica()
    {
        var resultado = _sut.Calcular("ANDRÉ");
        resultado.NumeroExpressao.Should().Be(1);
        resultado.DividasCarmicas.Should().Contain(19);
    }

    [Fact]
    public void Calcular_AndreComAcento_EhDiferenteDe_AndreSemAcento()
    {
        // O acento muda o valor: E(5) vs É(7) — resultados diferentes
        var semAcento = _sut.Calcular("ANDRE");
        var comAcento = _sut.Calcular("ANDRÉ");
        comAcento.NumeroExpressao.Should().NotBe(semAcento.NumeroExpressao);
        comAcento.NumeroMotivacao.Should().NotBe(semAcento.NumeroMotivacao);
    }

    // ── Nome composto ───────────────────────────────────────────────────────
    // Espaços são ignorados no cálculo

    [Fact]
    public void Calcular_NomeComposto_EspacoIgnorado()
    {
        var comEspaco    = _sut.Calcular("MARIA SILVA");
        var semEspaco    = _sut.Calcular("MARIASILVA");
        comEspaco.NumeroExpressao.Should().Be(semEspaco.NumeroExpressao);
        comEspaco.NumeroMotivacao.Should().Be(semEspaco.NumeroMotivacao);
        comEspaco.NumeroImpressao.Should().Be(semEspaco.NumeroImpressao);
    }

    // ── Dívidas Cármicas ────────────────────────────────────────────────────
    // Detectadas quando a soma intermediária (antes da redução final) é 13, 14, 16 ou 19

    [Fact]
    public void Calcular_SemDividaCármica_ListaVazia()
    {
        var resultado = _sut.Calcular("MARIA"); // soma=9, sem intermediário problemático
        resultado.DividasCarmicas.Should().BeEmpty();
    }

    [Fact]
    public void Calcular_ComDividaCármica13_Detectada()
    {
        // Precisamos de um nome cuja soma bruta seja 13
        // S(3) O(7) L(3) = 13 → Dívida 13 → reduz para 4
        var resultado = _sut.Calcular("SOL");
        resultado.DividasCarmicas.Should().Contain(13);
        resultado.NumeroExpressao.Should().Be(4);
    }
}
