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

        // RelacaoIntervalores = Impressão - Motivação = 4 - 3 = 1
        mapa.RelacaoIntervalores.Should().Be(1);
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
