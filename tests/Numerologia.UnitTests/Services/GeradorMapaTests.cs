using FluentAssertions;
using Numerologia.Core.Services;

namespace Numerologia.UnitTests.Services;

public class GeradorMapaTests
{
    private readonly GeradorMapa _gerador = new();

    // Usando nome simples com resultados verificáveis
    // Nome: "JOSE" → J=1, O=7, S=3, E=5
    // Vogais: O(7)+E(5)=12→3     Motivação=3
    // Consoantes: J(1)+S(3)=4    Impressão=4
    // Expressão: 12+4=16→7       (16 é Dívida Cármica!)
    // Data: 10/03/1985 → dia=10→1, mês=3, ano=1985→1+9+8+5=23→5
    // Destino: 1+3+5=9
    [Fact]
    public void Gerar_ComNomeEData_RetornaMapaComNumerosDoNomeCorretos()
    {
        var mapa = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));

        mapa.NumeroMotivacao.Should().Be(3);
        mapa.NumeroImpressao.Should().Be(4);
        mapa.NumeroExpressao.Should().Be(7);
        mapa.DividasCarmicas.Should().Contain(16);
    }

    [Fact]
    public void Gerar_ComNomeEData_RetornaMapaComNumerosDeDestinoCorretos()
    {
        var mapa = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));

        mapa.NumeroDestino.Should().Be(9);
        mapa.MesNascimentoReduzido.Should().Be(3);
        mapa.DiaNascimentoReduzido.Should().Be(1);
        mapa.AnoNascimentoReduzido.Should().Be(5);
    }

    [Fact]
    public void Gerar_ComNomeEData_PreencheRelacaoIntervalores()
    {
        var mapa = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));

        // RI = max do primeiro nome "JOSE": J(1) O(7) S(3) E(5) → max=7 → RI=7
        mapa.RelacaoIntervalores.Should().Be(7);
    }

    [Fact]
    public void Gerar_ComNomeEData_PreencheTabelas()
    {
        var mapa = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));

        mapa.DiasMesFavoraveis.Should().NotBeEmpty();
        mapa.NumerosHarmonicos.Should().NotBeEmpty();
        mapa.CoresFavoraveis.Should().NotBeEmpty();
        mapa.HarmoniaVibraCom.Should().BeInRange(1, 9);
    }

    [Fact]
    public void Gerar_DefineCriadoEmComoUtcNow()
    {
        var antes = DateTime.UtcNow;
        var mapa  = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));
        var depois = DateTime.UtcNow;

        mapa.CriadoEm.Should().BeOnOrAfter(antes).And.BeOnOrBefore(depois);
    }

    [Fact]
    public void Gerar_AtribuiConsulenteIdENomeUtilizado()
    {
        var mapa = _gerador.Gerar(consulenteId: 99, "JOSE", new DateOnly(1985, 3, 10));

        mapa.ConsulenteId.Should().Be(99);
        mapa.NomeUtilizado.Should().Be("JOSE");
        mapa.DataNascimento.Should().Be(new DateOnly(1985, 3, 10));
    }

    // ── Dívidas Cármicas — fontes completas (pág. 117) ──────────────────────
    // Motivação e Expressão: verificadas no CalculoMapa (valores finais 4/5/7/1)
    // Dia de nascimento: se 13/14/16/19 → dívida direta
    // Destino: se 4/5/7/1 → dívida correspondente

    [Fact]
    public void Gerar_DiaNascimento13_GeraDivida13()
    {
        // Dia 13 → Dívida 13 direto pelo dia (independente do nome)
        // Usar nome "ANA" (Motivação=2, Expressão=7→Dívida16) para isolar outras dívidas
        // Resultado esperado: [13, 16]
        var mapa = _gerador.Gerar(1, "ANA", new DateOnly(1990, 5, 13));
        mapa.DividasCarmicas.Should().Contain(13);
    }

    [Fact]
    public void Gerar_Destino4_GeraDivida13()
    {
        // Precisamos de uma data cujo Destino reduza para 4
        // 01/01/1993 → 1+1+1+9+9+3=24 → 2+4=6. Não.
        // 01/02/1991 → 1+2+1+9+9+1=23 → 5. Não.
        // 04/04/1985 → 4+4+1+9+8+5=31 → 4. Sim!
        // Usar "MARIA" (Motivação=3, Expressão=9 → sem dívidas do nome)
        var mapa = _gerador.Gerar(1, "MARIA", new DateOnly(1985, 4, 4));
        mapa.DividasCarmicas.Should().Contain(13);
    }

    // ── Harmonia Conjugal — deve usar Missão (= Expressão + Destino), não Expressão (pág. 205) ──

    [Fact]
    public void Gerar_HarmoniaUsaMissao_NaoExpressao()
    {
        // "ANA": Expressão=7, Data=15/08/1990 → Destino=6, Missão=Reduzir(7+6)=4
        // Tabela[4].VibraCom = 6; Tabela[7].VibraCom = 3
        // Se usar Expressão (errado) → VibraCom=3; se usar Missão (correto) → VibraCom=6
        var mapa = _gerador.Gerar(1, "ANA", new DateOnly(1990, 8, 15));

        mapa.Missao.Should().Be(4);
        mapa.HarmoniaVibraCom.Should().Be(6); // tabela[Missão=4]
    }

    // ── Atualizar ────────────────────────────────────────────────────────────

    [Fact]
    public void Atualizar_AlteraNomeERecalculaNumeros()
    {
        var mapa = _gerador.Gerar(1, "JOSE", new DateOnly(1985, 3, 10));

        // "ANA": A=1, N=5, A=1 → Motivação=2, Impressão=5, Expressão=7
        _gerador.Atualizar(mapa, "ANA");

        mapa.NomeUtilizado.Should().Be("ANA");
        mapa.NumeroMotivacao.Should().Be(2);
        mapa.NumeroImpressao.Should().Be(5);
        mapa.NumeroExpressao.Should().Be(7);
    }

    [Fact]
    public void Atualizar_MantemDataNascimentoEConsulenteId()
    {
        var dataNasc = new DateOnly(1985, 3, 10);
        var mapa = _gerador.Gerar(consulenteId: 5, "JOSE", dataNasc);

        _gerador.Atualizar(mapa, "ANA");

        mapa.DataNascimento.Should().Be(dataNasc);
        mapa.ConsulenteId.Should().Be(5);
    }
}
