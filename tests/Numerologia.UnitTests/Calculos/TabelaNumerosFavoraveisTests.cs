using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class TabelaNumerosFavoraveisTests
{
    // Tabela pág. 216-218 — chave: (dia, mês) de nascimento

    public static TheoryData<int, int, int[]> AmostraPrimeiroDiaPorMes => new()
    {
        {  1,  1, [1, 5] },   // Jan 01
        {  1,  2, [2, 7] },   // Fev 01
        {  1,  3, [1, 7] },   // Mar 01
        {  1,  4, [1, 7] },   // Abr 01
        {  1,  5, [1, 2] },   // Mai 01
        {  1,  6, [1, 5] },   // Jun 01
        {  1,  7, [1, 2] },   // Jul 01
        {  1,  8, [1, 2] },   // Ago 01
        {  1,  9, [1, 5] },   // Set 01
        {  1, 10, [2, 7] },   // Out 01
        {  1, 11, [1, 7] },   // Nov 01
        {  1, 12, [1, 7] },   // Dez 01
    };

    [Theory]
    [MemberData(nameof(AmostraPrimeiroDiaPorMes))]
    public void Consultar_PrimeiroDia_RetornaNumerosFavoraveisCorretos(int dia, int mes, int[] esperados)
    {
        var r = TabelaNumerosFavoraveis.Consultar(dia, mes);

        r.Should().Equal(esperados);
    }

    public static TheoryData<int, int, int[]> AmostraUltimoDiaPorMes => new()
    {
        { 31,  1, [2, 7] },   // Jan 31
        { 29,  2, [6, 7] },   // Fev 29 (ano bissexto)
        { 31,  3, [1, 7] },   // Mar 31
        { 30,  4, [3, 6] },   // Abr 30 (último dia)
        { 31,  5, [1, 5] },   // Mai 31
        { 30,  6, [2, 3] },   // Jun 30 (último dia)
        { 31,  7, [1, 7] },   // Jul 31
        { 31,  8, [1, 5] },   // Ago 31
        { 30,  9, [3, 6] },   // Set 30 (último dia)
        { 31, 10, [1, 7] },   // Out 31
        { 30, 11, [3, 6] },   // Nov 30 (último dia)
        { 31, 12, [1, 3] },   // Dez 31
    };

    [Theory]
    [MemberData(nameof(AmostraUltimoDiaPorMes))]
    public void Consultar_UltimoDia_RetornaNumerosFavoraveisCorretos(int dia, int mes, int[] esperados)
    {
        var r = TabelaNumerosFavoraveis.Consultar(dia, mes);

        r.Should().Equal(esperados);
    }

    public static TheoryData<int, int, int[]> AmostraValoresEspecificos => new()
    {
        {  9,  1, [6, 9] },   // Jan 09
        { 12,  2, [5, 6] },   // Fev 12
        { 28,  3, [5, 9] },   // Mar 28
        { 24,  4, [3, 5] },   // Abr 24
        { 17,  5, [2, 3] },   // Mai 17
        { 11,  6, [5, 7] },   // Jun 11
        { 15,  7, [6, 7] },   // Jul 15
        { 15,  8, [1, 6] },   // Ago 15
        { 27,  9, [6, 9] },   // Set 27
        { 10, 10, [1, 5] },   // Out 10
        { 06, 11, [3, 5] },   // Nov 06
        { 25, 12, [3, 7] },   // Dez 25
    };

    [Theory]
    [MemberData(nameof(AmostraValoresEspecificos))]
    public void Consultar_ValoresEspecificos_RetornaNumerosFavoraveisCorretos(int dia, int mes, int[] esperados)
    {
        var r = TabelaNumerosFavoraveis.Consultar(dia, mes);

        r.Should().Equal(esperados);
    }

    [Theory]
    [InlineData(0,  1)]    // dia inválido
    [InlineData(32, 1)]    // dia inválido
    [InlineData(31, 4)]    // Abril não tem dia 31
    [InlineData(30, 2)]    // Fevereiro não tem dia 30
    [InlineData(1,  0)]    // mês inválido
    [InlineData(1, 13)]    // mês inválido
    public void Consultar_DataInvalida_LancaArgumentOutOfRangeException(int dia, int mes)
    {
        var act = () => TabelaNumerosFavoraveis.Consultar(dia, mes);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    // ── GerarDiasFavoraveis (pág. 202) ──────────────────────────────────────
    // Algoritmo: a, b, 2b, depois alternando +a e +b até > 31.

    [Fact]
    public void GerarDiasFavoraveis_ExemploDoLivro_21Fevereiro()
    {
        // Base (21/fev) = [3, 6]
        // 3, 6, 12(=6×2), 15(+3), 21(+6), 24(+3), 30(+6), 33>31
        var r = TabelaNumerosFavoraveis.GerarDiasFavoraveis(21, 2);

        r.Should().Equal([3, 6, 12, 15, 21, 24, 30]);
    }

    [Fact]
    public void GerarDiasFavoraveis_Base2e7()
    {
        // 2, 7, 14(=7×2), 16(+2), 23(+7), 25(+2), 32>31
        var r = TabelaNumerosFavoraveis.GerarDiasFavoraveis(1, 2); // base=[2,7]

        r.Should().Equal([2, 7, 14, 16, 23, 25]);
    }

    [Fact]
    public void GerarDiasFavoraveis_Base6e9()
    {
        // 6, 9, 18(=9×2), 24(+6), 33>31
        var r = TabelaNumerosFavoraveis.GerarDiasFavoraveis(9, 1); // base=[6,9]

        r.Should().Equal([6, 9, 18, 24]);
    }

    [Fact]
    public void GerarDiasFavoraveis_Base1e5()
    {
        // 1, 5, 10(=5×2), 11(+1), 16(+5), 17(+1), 22(+5), 23(+1), 28(+5), 29(+1), 34>31
        var r = TabelaNumerosFavoraveis.GerarDiasFavoraveis(13, 1); // base=[1,5]

        r.Should().Equal([1, 5, 10, 11, 16, 17, 22, 23, 28, 29]);
    }

    [Fact]
    public void GerarDiasFavoraveis_NaoUltrapassaDia31()
    {
        // Qualquer par de base deve gerar apenas dias ≤ 31
        var r = TabelaNumerosFavoraveis.GerarDiasFavoraveis(28, 7); // base=[2,7]

        r.Should().OnlyContain(d => d >= 1 && d <= 31);
    }
}
