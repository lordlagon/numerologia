using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoDestinoTests
{
    private readonly CalculoDestino _sut = new();

    // ── Destino — exemplos do livro (pág. 96–97) ────────────────────────────
    // Soma de todos os dígitos da data → reduzir
    // 11 e 22 não se reduzem

    [Theory]
    [InlineData(1960, 6, 11, 6)]  // 1+1+6+1+9+6+0=24 → 2+4=6
    [InlineData(1972, 2, 28, 4)]  // 2+8+2+1+9+7+2=31 → 3+1=4  (livro: 29/02/1971 — data inválida, mesma soma)
    [InlineData(1960, 1,  3, 2)]  // 3+1+1+9+6+0=20   → 2+0=2
    public void Calcular_Destino_ExemplosDoLivro(int ano, int mes, int dia, int esperado)
    {
        var data = new DateOnly(ano, mes, dia);
        _sut.Calcular(data, numeroExpressao: 1).NumeroDestino.Should().Be(esperado);
    }

    [Fact]
    public void Calcular_Destino_NumeroMestreNaoReduz()
    {
        // Precisamos de uma data cuja soma seja 11 ou 22
        // 29/09/1958: 2+9+9+1+9+5+8=43 → 4+3=7 (não mestre)
        // 29/02/1948: 2+9+2+1+9+4+8=35 → 3+5=8 (não)
        // Tentando: 11/09/1967: 1+1+9+1+9+6+7=34→7 (não)
        // 11/11/2000: 1+1+1+1+2+0+0+0=6 (não)
        // 02/09/1970: 2+9+1+9+7+0=28→10→1 (não)
        // 11/09/1948: 1+1+9+1+9+4+8=33→6 (não)
        // Vamos usar: 29/06/1947: 2+9+6+1+9+4+7=38→11 ← mestre!
        var data = new DateOnly(1947, 6, 29);
        _sut.Calcular(data, numeroExpressao: 1).NumeroDestino.Should().Be(11);
    }

    // ── Missão = Destino + Expressão → reduzir ───────────────────────────────

    [Fact]
    public void Calcular_Missao_SomaDestinoMaisExpressao()
    {
        // Data 11/06/1960 → Destino=6; Expressão=8 → Missão=6+8=14→1+4=5
        var data = new DateOnly(1960, 6, 11);
        var r = _sut.Calcular(data, numeroExpressao: 8);
        r.Missao.Should().Be(5);
    }

    [Fact]
    public void Calcular_Missao_NumeroMestreMantido()
    {
        // Destino=2, Expressão=9 → 2+9=11 → mestre
        var data = new DateOnly(1960, 1, 3); // Destino=2
        var r = _sut.Calcular(data, numeroExpressao: 9);
        r.Missao.Should().Be(11);
    }

    // ── Componentes reduzidos da data (para Fig. D e cálculos de Ciclos) ────

    [Fact]
    public void Calcular_ComponentesDaData_Reduzidos()
    {
        // Data: 11/06/1960
        // Mês: 6 → 6
        // Dia: 11 → 1+1=2
        // Ano: 1960 → 1+9+6+0=16 → 1+6=7
        var data = new DateOnly(1960, 6, 11);
        var r = _sut.Calcular(data, numeroExpressao: 1);
        r.MesReduzido.Should().Be(6);
        r.DiaReduzido.Should().Be(2);
        r.AnoReduzido.Should().Be(7);
    }

    [Fact]
    public void Calcular_ComponentesDaData_MestreMantido()
    {
        // Dia 29 → 2+9=11 → mestre
        var data = new DateOnly(1947, 6, 29);
        var r = _sut.Calcular(data, numeroExpressao: 1);
        r.DiaReduzido.Should().Be(11);
    }
}
