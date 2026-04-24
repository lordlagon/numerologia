using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculosPessoaisTests
{
    private readonly CalculosPessoais _sut = new();

    // Data de nascimento: 11/06/1960
    // Ano atual: 2024
    // Ano Pessoal = Dia(11→1+1=2) + Mês(6) + AnoAtual(2024→2+0+2+4=8) = 2+6+8=16→7
    // Mês atual: 3 (março)
    // Mês Pessoal = AnoPessoal(7) + MêsAtual(3) = 10→1
    // Dia atual: 15
    // Dia Pessoal = MêsPessoal(1) + DiaAtual(15→1+5=6) = 1+6=7

    [Fact]
    public void Calcular_AnoPessoal_SomaDiaMesAnoAtual()
    {
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual: new DateOnly(2024, 3, 15));

        r.AnoPessoal.Should().Be(7);
    }

    [Fact]
    public void Calcular_MesPessoal_SomaAnoPessoalMesAtual()
    {
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual: new DateOnly(2024, 3, 15));

        r.MesPessoal.Should().Be(1);
    }

    [Fact]
    public void Calcular_DiaPessoal_SomaMesPessoalDiaAtual()
    {
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual: new DateOnly(2024, 3, 15));

        r.DiaPessoal.Should().Be(7);
    }

    [Fact]
    public void Calcular_AnoPessoal_NumeroMestreMantido()
    {
        // Nascimento: 29/02/1972
        // Dia(29→2+9=11 → mestre!) + Mês(2) + AnoAtual(2025→2+0+2+5=9) = 11+2+9=22 → mestre!
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1972, 2, 29),
            dataAtual: new DateOnly(2025, 1, 1));

        r.AnoPessoal.Should().Be(22);
    }

    [Fact]
    public void Calcular_AnoPessoal_OutroExemplo()
    {
        // Nascimento: 07/09/1985
        // Dia(7) + Mês(9) + AnoAtual(2026→2+0+2+6=10→1) = 7+9+1=17→8
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1985, 9, 7),
            dataAtual: new DateOnly(2026, 6, 20));

        r.AnoPessoal.Should().Be(8);
    }
}
