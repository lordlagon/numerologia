using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoDesafiosTests
{
    private readonly CalculoDestino _sut = new();

    // Data base: 11/06/1960
    // Mês=6, Dia=2 (11→2), Ano=7 (1960→16→7)
    // 1º Desafio = |Mês − Dia| = |6−2| = 4
    // 2º Desafio = |Ano − Dia| = |7−2| = 5
    // Principal  = |2º − 1º|  = |5−4| = 1

    [Fact]
    public void Calcular_Desafio1_DiferencaMesMenosDia()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.Desafio1.Should().Be(4);
    }

    [Fact]
    public void Calcular_Desafio2_DiferencaAnoMenosDia()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.Desafio2.Should().Be(5);
    }

    [Fact]
    public void Calcular_DesafioPrincipal_DiferencaDesafio2MenosDesafio1()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.DesafioPrincipal.Should().Be(1);
    }

    [Fact]
    public void Calcular_DesafioZero_QuandoDiferencaEhZero()
    {
        // Mês=3 Dia=3 → 1º=0. Exemplo: 03/03/1980
        // Mês=3, Dia=3, Ano=1980→18→9
        // 1º Desafio=|3−3|=0, 2º=|9−3|=6, Principal=|6−0|=6
        var r = _sut.Calcular(new DateOnly(1980, 3, 3), numeroExpressao: 1);
        r.Desafio1.Should().Be(0);
        r.Desafio2.Should().Be(6);
        r.DesafioPrincipal.Should().Be(6);
    }

    [Fact]
    public void Calcular_Desafio_SemprePositivo()
    {
        // |Mês − Dia| deve ser valor absoluto — nunca negativo
        // Mês=2 Dia=9 → |2−9|=7 (não −7)
        // Data: 09/02/1990 → Mês=2, Dia=9, Ano=1990→19→1+9=10→1
        var r = _sut.Calcular(new DateOnly(1990, 2, 9), numeroExpressao: 1);
        r.Desafio1.Should().Be(7);
    }

    // ── Números mestres nos desafios devem ser reduzidos: 11→2 e 22→4 ─────────
    // (os ciclos preservam 11/22, mas a Fig. H usa redução total)

    [Fact]
    public void Calcular_Desafio_OnzeReduzidoParaDoisNaFigH()
    {
        // 29/06/1947: DiaReduzido=11 (mestre), MesReduzido=6, AnoReduzido=2
        // Para desafios: 11→2
        // Desafio1 = |6 − 2| = 4
        var r = _sut.Calcular(new DateOnly(1947, 6, 29), numeroExpressao: 1);
        r.Desafio1.Should().Be(4);
    }

    [Fact]
    public void Calcular_Desafio2_VinteDoisReduzidoParaQuatroNaFigH()
    {
        // André 28/07/1984: DiaReduzido=1, MesReduzido=7, AnoReduzido=22 (mestre)
        // Para desafios: 22→4
        // Desafio2 = |4 − 1| = 3
        var r = _sut.Calcular(new DateOnly(1984, 7, 28), numeroExpressao: 1);
        r.Desafio2.Should().Be(3);
    }
}
