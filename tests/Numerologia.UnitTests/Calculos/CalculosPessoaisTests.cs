using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class CalculosPessoaisTests
{
    private readonly CalculosPessoais _sut = new();

    // ── Exemplos diretos do livro (pág. 176) ────────────────────────────────

    [Fact]
    public void AnoPessoal_LivroExemplo1_AniversarioJaPassou()
    {
        // Nascido 28/jan, cálculo 19/jun/2004 — aniversário JÁ passou → usa 2004
        // SomarDigitos(28)=10, mês=1, SomarDigitos(2004)=6 → 10+1+6=17=8
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 1, 28),
            dataAtual:      new DateOnly(2004, 6, 19));

        r.AnoPessoal.Should().Be(8);
    }

    [Fact]
    public void AnoPessoal_LivroExemplo2_AniversarioAindaNaoPassou()
    {
        // Nascido 18/nov, cálculo 19/jun/2004 — aniversário AINDA NÃO passou → usa 2003
        // SomarDigitos(18)=9, mês=11, SomarDigitos(2003)=5 → 9+11+5=25=7
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 11, 18),
            dataAtual:      new DateOnly(2004, 6, 19));

        r.AnoPessoal.Should().Be(7);
    }

    // ── Ano Pessoal — regra do aniversário ──────────────────────────────────

    [Fact]
    public void AnoPessoal_AniversarioNoMesmoDia_UsaAnoCorrente()
    {
        // Nascido 15/03, cálculo 15/03/2024 — aniversário É hoje → considera passado → usa 2024
        // SomarDigitos(15)=6, mês=3, SomarDigitos(2024)=8 → 6+3+8=17=8
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1990, 3, 15),
            dataAtual:      new DateOnly(2024, 3, 15));

        r.AnoPessoal.Should().Be(8);
    }

    [Fact]
    public void AnoPessoal_AniversarioNaoPassou_UsaAnoAnterior()
    {
        // Nascido 11/06/1960, cálculo 15/03/2024 — 11/jun ainda não passou em março → usa 2023
        // SomarDigitos(11)=2, mês=6, SomarDigitos(2023)=7 → 2+6+7=15=6
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual:      new DateOnly(2024, 3, 15));

        r.AnoPessoal.Should().Be(6);
    }

    [Fact]
    public void AnoPessoal_AndreExemplo_AniversarioNaoPassou()
    {
        // André: nascido 28/07/1984, criação do mapa 25/04/2026
        // 28/jul ainda não passou em abril → usa 2025
        // SomarDigitos(28)=10, mês=7, SomarDigitos(2025)=9 → 10+7+9=26=8
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1984, 7, 28),
            dataAtual:      new DateOnly(2026, 4, 25));

        r.AnoPessoal.Should().Be(8);
    }

    [Fact]
    public void AnoPessoal_NumeroMestreMantido()
    {
        // Nascido 29/02/1972, cálculo 01/03/2025
        // 29/fev→ em 2025 não existe; comparando mês: mês2 < mês3 → considera passado → usa 2025
        // SomarDigitos(29)=11 (mestre), mês=2, SomarDigitos(2025)=9 → 11+2+9=22 (mestre!)
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1972, 2, 29),
            dataAtual:      new DateOnly(2025, 3, 1));

        r.AnoPessoal.Should().Be(22);
    }

    // ── Mês Pessoal ─────────────────────────────────────────────────────────
    // Fórmula: Reduzir(AnoPessoal + mêsNascimento)

    [Fact]
    public void MesPessoal_UsaMesNascimento_NaoMesAtual()
    {
        // André: AnoPessoal=8, mêsNascimento=7 → 8+7=15=6
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1984, 7, 28),
            dataAtual:      new DateOnly(2026, 4, 25));

        r.MesPessoal.Should().Be(6);
    }

    [Fact]
    public void MesPessoal_Nascido1106_Calculo15032024()
    {
        // AnoPessoal=6 (usa 2023), mêsNascimento=6 → 6+6=12=3
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual:      new DateOnly(2024, 3, 15));

        r.MesPessoal.Should().Be(3);
    }

    // ── Dia Pessoal ─────────────────────────────────────────────────────────
    // Fórmula: Reduzir(MêsPessoal + SomarDigitos(diaAtual))

    [Fact]
    public void DiaPessoal_AndreExemplo()
    {
        // André: MêsPessoal=6, diaAtual=25 → SomarDigitos(25)=7 → 6+7=13=4
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1984, 7, 28),
            dataAtual:      new DateOnly(2026, 4, 25));

        r.DiaPessoal.Should().Be(4);
    }

    [Fact]
    public void DiaPessoal_Nascido1106_Calculo15032024()
    {
        // MêsPessoal=3, diaAtual=15 → SomarDigitos(15)=6 → 3+6=9
        var r = _sut.Calcular(
            dataNascimento: new DateOnly(1960, 6, 11),
            dataAtual:      new DateOnly(2024, 3, 15));

        r.DiaPessoal.Should().Be(9);
    }
}
