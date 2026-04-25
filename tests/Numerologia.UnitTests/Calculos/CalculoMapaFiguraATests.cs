using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoMapaFiguraATests
{
    private readonly CalculoMapa _sut = new();

    // ── MARIA ──────────────────────────────────────────────────────────────
    // M(4) A(1) R(2) I(1) A(1)
    // FigA: {1→3, 2→1, 4→1} — valores 3,5,6,7,8,9 ausentes
    // Lições Cármicas: 3, 5, 6, 7, 8, 9  (ausentes de 1-9)
    // Tendência Oculta: [1]  (aparece 3 vezes)
    // Resposta Subconsciente: 9 − 6 = 3

    [Fact]
    public void Calcular_Maria_FiguraA_ContagemCorreta()
    {
        var r = _sut.Calcular("MARIA");
        r.FiguraA[1].Should().Be(3); // A, I, A
        r.FiguraA[2].Should().Be(1); // R
        r.FiguraA[4].Should().Be(1); // M
        r.FiguraA[3].Should().Be(0);
        r.FiguraA[5].Should().Be(0);
        r.FiguraA[6].Should().Be(0);
        r.FiguraA[7].Should().Be(0);
        r.FiguraA[8].Should().Be(0);
        r.FiguraA[9].Should().Be(0);
    }

    [Fact]
    public void Calcular_Maria_LicoesCarmicas()
    {
        var r = _sut.Calcular("MARIA");
        r.LicoesCarmicas.Should().BeEquivalentTo([3, 5, 6, 7, 8, 9]);
    }

    [Fact]
    public void Calcular_Maria_TendenciaOculta()
    {
        var r = _sut.Calcular("MARIA");
        r.TendenciasOcultas.Should().BeEquivalentTo([1]);
    }

    [Fact]
    public void Calcular_Maria_RespostaSubconsciente()
    {
        var r = _sut.Calcular("MARIA");
        r.RespostaSubconsciente.Should().Be(3); // 9 - 6 lições
    }

    // ── CARLOS ─────────────────────────────────────────────────────────────
    // C(3) A(1) R(2) L(3) O(7) S(3)
    // FigA: {1→1, 2→1, 3→3, 7→1} — valores 4,5,6,8,9 ausentes
    // Lições Cármicas: 4, 5, 6, 8, 9  (ausentes de 1-9)
    // Tendência Oculta: [3]
    // Resposta Subconsciente: 9 − 5 = 4

    [Fact]
    public void Calcular_Carlos_FiguraA_ContagemCorreta()
    {
        var r = _sut.Calcular("CARLOS");
        r.FiguraA[1].Should().Be(1); // A
        r.FiguraA[2].Should().Be(1); // R
        r.FiguraA[3].Should().Be(3); // C, L, S
        r.FiguraA[7].Should().Be(1); // O
        r.FiguraA[4].Should().Be(0);
        r.FiguraA[5].Should().Be(0);
        r.FiguraA[6].Should().Be(0);
        r.FiguraA[8].Should().Be(0);
    }

    [Fact]
    public void Calcular_Carlos_LicoesCarmicas()
    {
        var r = _sut.Calcular("CARLOS");
        r.LicoesCarmicas.Should().BeEquivalentTo([4, 5, 6, 8, 9]);
    }

    [Fact]
    public void Calcular_Carlos_TendenciaOculta()
    {
        var r = _sut.Calcular("CARLOS");
        r.TendenciasOcultas.Should().BeEquivalentTo([3]);
    }

    [Fact]
    public void Calcular_Carlos_RespostaSubconsciente()
    {
        var r = _sut.Calcular("CARLOS");
        r.RespostaSubconsciente.Should().Be(4);
    }

    // ── Empate na Tendência Oculta ──────────────────────────────────────────
    // ANA: A(1) N(5) A(1) → {1→2, 5→1}
    // Tendência: [1] (única com maior contagem)
    // NANA: N(5) A(1) N(5) A(1) → {1→2, 5→2} → empate → ambas são tendências

    [Fact]
    public void Calcular_Empate_TodasAsTendenciasRetornadas()
    {
        var r = _sut.Calcular("NANA");
        r.TendenciasOcultas.Should().BeEquivalentTo([1, 5]);
    }

    // ── Nome sem Lições Cármicas ────────────────────────────────────────────
    // Nome que cobre todos os valores 1-9 → sem lições
    // Ó = O(7) + agudo(+2) = 9

    [Fact]
    public void Calcular_NomeComTodosOsValores_SemLicoes()
    {
        // A(1) B(2) C(3) D(4) E(5) U(6) O(7) F(8) Ó(9)
        var r = _sut.Calcular("ABCDEOUOF Ó");
        r.LicoesCarmicas.Should().BeEmpty();
        r.RespostaSubconsciente.Should().Be(9); // 9 − 0 lições
    }
}
