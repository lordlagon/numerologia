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

    // ── Relação Intervalores (pág. 203) ─────────────────────────────────────

    [Fact]
    public void Calcular_Maria_RelacaoIntervalores()
    {
        // MARIA: M(4) A(1) R(2) I(1) A(1) → max=4 → RI=4
        var resultado = _sut.Calcular("MARIA");
        resultado.RelacaoIntervalores.Should().Be(4);
    }

    [Fact]
    public void Calcular_NomeComposto_RelacaoIntervaloresUsaPrimeiroNome()
    {
        // "JOSE DA SILVA": primeiro nome JOSE → J(1) O(7) S(3) E(5) → max=7 → RI=7
        // Ignora DA(4,1) e SILVA(3,1,3,6,1) que são sobrenomes
        var resultado = _sut.Calcular("JOSE DA SILVA");
        resultado.RelacaoIntervalores.Should().Be(7);
    }

    // ── Dívidas Cármicas (pág. 117) ─────────────────────────────────────────
    // Regra do livro: verificar os valores FINAIS REDUZIDOS de Motivação e Expressão.
    // Se forem 4, 5, 7 ou 1 → indicadores de Dívidas 13, 14, 16 e 19 respectivamente.
    // Dia de nascimento e Destino são verificados no GeradorMapa (requerem data).

    [Fact]
    public void Calcular_SemDividaCármica_ListaVazia()
    {
        // MARIA: Motivação=3, Expressão=9 — nenhum é 4/5/7/1
        var resultado = _sut.Calcular("MARIA");
        resultado.DividasCarmicas.Should().BeEmpty();
    }

    [Fact]
    public void Calcular_ComDividaCármica13_Detectada()
    {
        // SOL: S(3)+O(7)+L(3)=13 → Expressão=4 → Dívida 13
        var resultado = _sut.Calcular("SOL");
        resultado.DividasCarmicas.Should().Contain(13);
        resultado.NumeroExpressao.Should().Be(4);
    }

    // ── Tendências Ocultas ──────────────────────────────────────────────────
    // Regra pág. 114: só há Tendência Oculta se o valor aparecer MAIS de 3 vezes (≥ 4).
    // Se nenhum número atingir esse limiar → lista vazia (sem fallback para o máximo).

    [Fact]
    public void TendenciasOcultas_SemNumeroAcimaDe3_RetornaVazio()
    {
        // MARIA: M(4)×1, A(1)×3, R(2)×1, I(1)×1 → máx = 3 (valor 1), mas 3 < 4
        // Resultado esperado: [] (nenhuma tendência oculta)
        var resultado = _sut.Calcular("MARIA");
        resultado.TendenciasOcultas.Should().BeEmpty();
    }

    [Fact]
    public void TendenciasOcultas_ComUmNumeroExatamenteCincoVezes_RetornaEsseNumero()
    {
        // Joaquim Cardoso (exemplo do livro): número 1 aparece 5x (J,A,Q,I,A) → Tendência = 1
        // J(1) O(7) A(1) Q(1) U(6) I(1) M(4)  C(3) A(1) R(2) D(4) O(7) S(3) O(7)
        // valor 1: J,A,Q,I,A = 5x → Tendência Oculta 1
        var resultado = _sut.Calcular("JOAQUIM CARDOSO");
        resultado.TendenciasOcultas.Should().Contain(1);
    }

    [Fact]
    public void TendenciasOcultas_ComMultiplosNumerosAcimaDe3_RetornaTodos()
    {
        // André Luiz Xavier de Macedo: 1→5x, 4→4x, 5→4x → todos ≥ 4
        var resultado = _sut.Calcular("André Luiz Xavier de Macedo");
        resultado.TendenciasOcultas.Should().BeEquivalentTo([1, 4, 5]);
    }
}
