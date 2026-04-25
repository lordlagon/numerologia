using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoMomentosDecisivosTests
{
    private readonly CalculoDestino _sut = new();

    // Data base: 11/06/1960
    // Mês=6, Dia=2 (11→2), Ano=7 (1960→16→7)
    // 1º MD = Dia + Mês    = 2+6 = 8
    // 2º MD = Dia + Ano    = 2+7 = 9
    // 3º MD = 1º + 2º      = 8+9 = 17 → 1+7 = 8
    // 4º MD = Mês + Ano    = 6+7 = 13 → 1+3 = 4

    [Fact]
    public void Calcular_MD1_DiaMaisMes()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.MomentoDecisivo1.Should().Be(8);
    }

    [Fact]
    public void Calcular_MD2_DiaMaisAno()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.MomentoDecisivo2.Should().Be(9);
    }

    [Fact]
    public void Calcular_MD3_MD1MaisMD2()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.MomentoDecisivo3.Should().Be(8); // 17→8
    }

    [Fact]
    public void Calcular_MD4_MesMaisAno()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.MomentoDecisivo4.Should().Be(4); // 13→4
    }

    [Fact]
    public void Calcular_MD_NumeroMestreMantido()
    {
        // Mês=6, Dia=7 (07→7), Ano=5 (1985→23→5)
        // MD1 = 7+6=13→4; MD2=7+5=12→3; MD3=4+3=7; MD4=6+5=11→mestre!
        var r = _sut.Calcular(new DateOnly(1985, 6, 7), numeroExpressao: 1);
        r.MomentoDecisivo4.Should().Be(11);
    }
}
