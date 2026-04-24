using FluentAssertions;
using Numerologia.Core.Calculos;
using Numerologia.Core.Entities;

namespace Numerologia.UnitTests.Entities;

public class MapaNumerologicoTests
{
    // Nome: "ANA" → A=1, N=5, A=1
    // Vogais: A+A=2 → Motivação=2
    // Consoantes: N=5 → Impressão=5
    // Expressão: 7
    // Data: 15/08/1990 → dia=15→6, mês=8, ano=1990→1+9+9+0=19→10→1
    // Destino: 6+8+1=15→6
    private static readonly ResultadoMapa _mapa = new(
        NumeroMotivacao:      2,
        NumeroImpressao:      5,
        NumeroExpressao:      7,
        DividasCarmicas:      [],
        FiguraA:              new Dictionary<int,int> { [1]=2,[2]=0,[3]=0,[4]=0,[5]=1,[6]=0,[7]=0,[8]=0,[9]=0 },
        LicoesCarmicas:       [2,3,4,6,7,8],
        TendenciasOcultas:    [1],
        RespostaSubconsciente: 6
    );

    private static readonly ResultadoDestino _destino = new(
        NumeroDestino:    6,
        Missao:           ReducaoNumerologica.Reduzir(6 + 7),
        MesReduzido:      8,
        DiaReduzido:      6,
        AnoReduzido:      1,
        CicloVida1:       8,
        CicloVida2:       6,
        CicloVida3:       1,
        FimCiclo1Idade:   30,
        FimCiclo2Idade:   57,
        Desafio1:         Math.Abs(8 - 6),
        Desafio2:         Math.Abs(1 - 6),
        DesafioPrincipal: Math.Abs(5 - 2),
        MomentoDecisivo1: ReducaoNumerologica.Reduzir(6 + 8),
        MomentoDecisivo2: ReducaoNumerologica.Reduzir(6 + 1),
        MomentoDecisivo3: ReducaoNumerologica.Reduzir(5 + 7),
        MomentoDecisivo4: ReducaoNumerologica.Reduzir(8 + 1)
    );

    private static readonly int[]    _diasFavoraveis = [1, 6, 15, 20];
    private static readonly int[]    _numerosHarmonicos = [2, 4, 5, 7];
    private static readonly string[] _cores = ["Verde", "Amarelo", "Branco"];
    private static readonly ResultadoHarmoniaConjugal _harmonia =
        new(VibraCom: 3, Atrai: [2, 6], EOpostoA: [1, 9], ProfundamenteOpostoA: [], EPassivoEm: [4, 5, 8]);

    [Fact]
    public void Criar_ComDadosValidos_DefineDadosBasicos()
    {
        var consulenteId = 42;
        var nome         = "ANA";
        var data         = new DateOnly(1990, 8, 15);

        var mapa = MapaNumerologico.Criar(
            consulenteId, nome, data, _mapa, _destino, _diasFavoraveis, _numerosHarmonicos, _cores, _harmonia);

        mapa.ConsulenteId.Should().Be(consulenteId);
        mapa.NomeUtilizado.Should().Be(nome);
        mapa.DataNascimento.Should().Be(data);
        mapa.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Criar_ComDadosDoNome_DefineNumerosDoNome()
    {
        var mapa = CriarMapaPadrao();

        mapa.NumeroMotivacao.Should().Be(_mapa.NumeroMotivacao);
        mapa.NumeroImpressao.Should().Be(_mapa.NumeroImpressao);
        mapa.NumeroExpressao.Should().Be(_mapa.NumeroExpressao);
        mapa.RespostaSubconsciente.Should().Be(_mapa.RespostaSubconsciente);
        mapa.LicoesCarmicas.Should().BeEquivalentTo(_mapa.LicoesCarmicas);
        mapa.TendenciasOcultas.Should().BeEquivalentTo(_mapa.TendenciasOcultas);
        mapa.DividasCarmicas.Should().BeEquivalentTo(_mapa.DividasCarmicas);
        mapa.FiguraA.Should().BeEquivalentTo(_mapa.FiguraA);
    }

    [Fact]
    public void Criar_ComDadosDaData_DefineNumerosDeDestino()
    {
        var mapa = CriarMapaPadrao();

        mapa.NumeroDestino.Should().Be(_destino.NumeroDestino);
        mapa.Missao.Should().Be(_destino.Missao);
        mapa.MesNascimentoReduzido.Should().Be(_destino.MesReduzido);
        mapa.DiaNascimentoReduzido.Should().Be(_destino.DiaReduzido);
        mapa.AnoNascimentoReduzido.Should().Be(_destino.AnoReduzido);
        mapa.CicloVida1.Should().Be(_destino.CicloVida1);
        mapa.CicloVida2.Should().Be(_destino.CicloVida2);
        mapa.CicloVida3.Should().Be(_destino.CicloVida3);
        mapa.FimCiclo1Idade.Should().Be(_destino.FimCiclo1Idade);
        mapa.FimCiclo2Idade.Should().Be(_destino.FimCiclo2Idade);
        mapa.Desafio1.Should().Be(_destino.Desafio1);
        mapa.Desafio2.Should().Be(_destino.Desafio2);
        mapa.DesafioPrincipal.Should().Be(_destino.DesafioPrincipal);
        mapa.MomentoDecisivo1.Should().Be(_destino.MomentoDecisivo1);
        mapa.MomentoDecisivo2.Should().Be(_destino.MomentoDecisivo2);
        mapa.MomentoDecisivo3.Should().Be(_destino.MomentoDecisivo3);
        mapa.MomentoDecisivo4.Should().Be(_destino.MomentoDecisivo4);
    }

    [Fact]
    public void Criar_ComTabelasFixas_DefineNumerosDeTabelas()
    {
        var mapa = CriarMapaPadrao();

        mapa.DiasMesFavoraveis.Should().BeEquivalentTo(_diasFavoraveis);
        mapa.NumerosHarmonicos.Should().BeEquivalentTo(_numerosHarmonicos);
        mapa.CoresFavoraveis.Should().BeEquivalentTo(_cores);
        mapa.RelacaoIntervalores.Should().Be(_mapa.NumeroImpressao - _mapa.NumeroMotivacao);
        mapa.HarmoniaVibraCom.Should().Be(_harmonia.VibraCom);
        mapa.HarmoniaAtrai.Should().BeEquivalentTo(_harmonia.Atrai);
        mapa.HarmoniaEOpostoA.Should().BeEquivalentTo(_harmonia.EOpostoA);
        mapa.HarmoniaProfundamenteOpostoA.Should().BeEquivalentTo(_harmonia.ProfundamenteOpostoA);
        mapa.HarmoniaEPassivoEm.Should().BeEquivalentTo(_harmonia.EPassivoEm);
    }

    private static MapaNumerologico CriarMapaPadrao() =>
        MapaNumerologico.Criar(1, "ANA", new DateOnly(1990, 8, 15),
            _mapa, _destino, _diasFavoraveis, _numerosHarmonicos, _cores, _harmonia);
}
