using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculoCiclosTests
{
    private readonly CalculoDestino _sut = new();

    // Data base: 11/06/1960  (Destino=6)
    // 1º Ciclo = mês reduzido           → 6
    // 2º Ciclo = dia reduzido           → 11→2
    // 3º Ciclo = ano reduzido           → 1960→16→7
    // Fim 1º Ciclo (idade) = 36−6 = 30
    // Fim 2º Ciclo (idade) = 30+27 = 57

    [Fact]
    public void Calcular_CicloVida1_EhMesReduzido()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.CicloVida1.Should().Be(6);
    }

    [Fact]
    public void Calcular_CicloVida2_EhDiaReduzido()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.CicloVida2.Should().Be(2);
    }

    [Fact]
    public void Calcular_CicloVida3_EhAnoReduzido()
    {
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.CicloVida3.Should().Be(7);
    }

    [Fact]
    public void Calcular_FimCiclo1_TrintaAnos()
    {
        // Destino=6 → 36−6=30
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.FimCiclo1Idade.Should().Be(30);
    }

    [Fact]
    public void Calcular_FimCiclo2_CinquentaESetaAnos()
    {
        // 30+27=57
        var r = _sut.Calcular(new DateOnly(1960, 6, 11), numeroExpressao: 1);
        r.FimCiclo2Idade.Should().Be(57);
    }

    [Fact]
    public void Calcular_CicloComMestreMantido()
    {
        // Dia 29 → 2+9=11 → mestre — o ciclo 2 deve ser 11
        var r = _sut.Calcular(new DateOnly(1947, 6, 29), numeroExpressao: 1);
        r.CicloVida2.Should().Be(11);
    }

    [Fact]
    public void Calcular_FimCiclo1_DestinoMestre()
    {
        // Destino=11 → 36−11=25
        var r = _sut.Calcular(new DateOnly(1947, 6, 29), numeroExpressao: 1);
        r.FimCiclo1Idade.Should().Be(25);
    }
}
