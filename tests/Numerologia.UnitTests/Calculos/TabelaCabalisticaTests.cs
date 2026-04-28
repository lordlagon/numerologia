using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class TabelaCabalisticaTests
{
    // Letras base — conforme tabela pág. 63
    [Theory]
    [InlineData('a', 1)] [InlineData('i', 1)] [InlineData('q', 1)]
    [InlineData('j', 1)] [InlineData('y', 1)]
    [InlineData('b', 2)] [InlineData('k', 2)] [InlineData('r', 2)]
    [InlineData('c', 3)] [InlineData('g', 3)] [InlineData('l', 3)]
    [InlineData('s', 3)]
    [InlineData('d', 4)] [InlineData('m', 4)] [InlineData('t', 4)]
    [InlineData('e', 5)] [InlineData('h', 5)] [InlineData('n', 5)]
    [InlineData('u', 6)] [InlineData('v', 6)] [InlineData('w', 6)]
    [InlineData('x', 6)] [InlineData('ç', 6)]
    [InlineData('o', 7)] [InlineData('z', 7)]
    [InlineData('f', 8)] [InlineData('p', 8)]
    public void ObterValor_LetraBase_RetornaValorCorreto(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Maiúsculas devem funcionar igual às minúsculas
    [Theory]
    [InlineData('A', 1)] [InlineData('B', 2)] [InlineData('O', 7)]
    [InlineData('F', 8)] [InlineData('Z', 7)]
    public void ObterValor_LetraMaiuscula_RetornaValorCorreto(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra ´ +2 (acento agudo)
    [Theory]
    [InlineData('á', 3)]  // a(1) + 2
    [InlineData('é', 7)]  // e(5) + 2
    [InlineData('í', 3)]  // i(1) + 2
    [InlineData('ó', 9)]  // o(7) + 2
    [InlineData('ú', 8)]  // u(6) + 2
    public void ObterValor_AcentoAgudo_RetornaBasemaisDois(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra ^ +7 (circunflexo), reduzido se > 9
    [Theory]
    [InlineData('â', 8)]  // a(1) + 7 = 8
    [InlineData('ê', 3)]  // e(5) + 7 = 12 → 3
    [InlineData('î', 8)]  // i(1) + 7 = 8
    [InlineData('ô', 5)]  // o(7) + 7 = 14 → 5
    [InlineData('û', 4)]  // u(6) + 7 = 13 → 4
    public void ObterValor_Circunflexo_RetornaBaseMaisSeteReduzido(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra ~ +3 (til), reduzido se > 9
    [Theory]
    [InlineData('ã', 4)]  // a(1) + 3 = 4
    [InlineData('õ', 1)]  // o(7) + 3 = 10 → 1
    public void ObterValor_Til_RetornaBaseMailsTresReduzido(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra ` x2 (acento grave), reduzido se > 9
    [Theory]
    [InlineData('à', 2)]  // a(1) × 2 = 2
    [InlineData('è', 1)]  // e(5) × 2 = 10 → 1
    public void ObterValor_AcentoGrave_RetornaBasePorDoisReduzido(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra .. x2 (trema), reduzido se > 9
    [Theory]
    [InlineData('ä', 2)]  // a(1) × 2 = 2
    [InlineData('ë', 1)]  // e(5) × 2 = 10 → 1
    [InlineData('ï', 2)]  // i(1) × 2 = 2
    [InlineData('ö', 5)]  // o(7) × 2 = 14 → 5
    [InlineData('ü', 3)]  // u(6) × 2 = 12 → 3
    public void ObterValor_Trema_RetornaBasePorDoisReduzido(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Diacríticos — regra o +7 (ring/bolinha acima — símbolo de assinatura), reduzido se > 9
    [Theory]
    [InlineData('å', 8)]  // a(1) + 7 = 8  (U+00E5)
    [InlineData('ů', 4)]  // u(6) + 7 = 13 → 4  (U+016F)
    public void ObterValor_Ring_RetornaBaseMaisSeteReduzido(char letra, int esperado)
    {
        TabelaCabalistica.ObterValor(letra).Should().Be(esperado);
    }

    // Ring via método explícito — letras que podem ter bolinha na assinatura: a, u, e, i
    // Não há Unicode composto para 'i com ring', então sempre via ObterValorComRing.
    [Theory]
    [InlineData('a', 8)]  // a(1) + 7 = 8   — pode ser digitado também como å (U+00E5)
    [InlineData('u', 4)]  // u(6) + 7 = 13 → 4 — pode ser digitado também como ů (U+016F)
    [InlineData('e', 3)]  // e(5) + 7 = 12 → 3 — pode ser digitado também como ė (U+0117)
    [InlineData('i', 8)]  // i(1) + 7 = 8   — sem Unicode composto, apenas via este método
    public void ObterValorComRing_AplicaRegra_MaisSeteReduzido(char letraBase, int esperado)
    {
        TabelaCabalistica.ObterValorComRing(letraBase).Should().Be(esperado);
    }

    // Caracteres sem valor (espaço, hífen, etc.) retornam 0
    [Theory]
    [InlineData(' ')] [InlineData('-')] [InlineData('1')]
    public void ObterValor_CaractereSemValor_RetornaZero(char c)
    {
        TabelaCabalistica.ObterValor(c).Should().Be(0);
    }

    // Vogais
    [Theory]
    [InlineData('a', true)]  [InlineData('e', true)]  [InlineData('i', true)]
    [InlineData('o', true)]  [InlineData('u', true)]  [InlineData('y', true)]
    [InlineData('á', true)]  [InlineData('é', true)]  [InlineData('í', true)]
    [InlineData('ó', true)]  [InlineData('ú', true)]
    [InlineData('â', true)]  [InlineData('ê', true)]  [InlineData('ô', true)]
    [InlineData('ã', true)]  [InlineData('õ', true)]
    [InlineData('à', true)]  [InlineData('è', true)]
    [InlineData('b', false)] [InlineData('c', false)] [InlineData('n', false)]
    public void EhVogal_RetornaCorreto(char letra, bool esperado)
    {
        TabelaCabalistica.EhVogal(letra).Should().Be(esperado);
    }
}
