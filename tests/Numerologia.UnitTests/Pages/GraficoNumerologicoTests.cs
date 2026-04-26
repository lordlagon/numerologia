using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Pages.Mapas;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class GraficoNumerologicoTests : TestContext
{
    private readonly IMapasService _mapasService = Substitute.For<IMapasService>();
    private readonly ICalculosPessoaisService _pessoaisService = Substitute.For<ICalculosPessoaisService>();

    private static readonly MapaDetalheDto _mapaFake = new(
        Id: 1,
        NomeUtilizado: "JOSE DA SILVA",
        DataNascimento: new DateOnly(1985, 3, 10),
        CriadoEm: DateTime.UtcNow,
        GradeLetras:
        [
            new("J", TipoLetraDto.Consoante, 1),
            new("O", TipoLetraDto.Vogal, 7),
            new("S", TipoLetraDto.Consoante, 3),
            new("E", TipoLetraDto.Vogal, 5),
        ],
        NumeroMotivacao: 3,
        NumeroImpressao: 4,
        NumeroExpressao: 7,
        SomaMotivacao: 12,
        SomaImpressao: 4,
        SomaExpressao: 16,
        DividasCarmicas: [],
        FiguraA: new() { ["1"]=1,["2"]=0,["3"]=1,["4"]=0,["5"]=1,["6"]=0,["7"]=1,["8"]=0,["9"]=0 },
        LicoesCarmicas: [2, 4, 6, 8],
        TendenciasOcultas: [1],
        RespostaSubconsciente: 4,
        MesNascimentoReduzido: 3,
        DiaNascimentoReduzido: 1,
        AnoNascimentoReduzido: 5,
        NumeroDestino: 9,
        Missao: 7,
        CicloVida1: 3, CicloVida2: 1, CicloVida3: 5,
        FimCiclo1Idade: 27, FimCiclo2Idade: 54,
        Desafio1: 2, Desafio2: 4, DesafioPrincipal: 2,
        MomentoDecisivo1: 4, MomentoDecisivo2: 6, MomentoDecisivo3: 1, MomentoDecisivo4: 8,
        DiasMesFavoraveis: [1, 10, 19, 28],
        NumerosHarmonicos: [1, 2, 3, 5, 6, 8, 9],
        RelacaoIntervalores: 7,
        HarmoniaVibraCom: 1,
        HarmoniaAtrai: [2, 3, 5, 6],
        HarmoniaEOpostoA: [7],
        HarmoniaProfundamenteOpostoA: [],
        HarmoniaEPassivoEm: [4, 8],
        CoresFavoraveis: ["Verde", "Amarelo", "Branco", "Cinza", "Azul-claro"]
    );

    private static readonly ResultadoPessoalDto _pessoalFake = new(AnoPessoal: 5, MesPessoal: 8, DiaPessoal: 3);

    public GraficoNumerologicoTests()
    {
        Services.AddSingleton(_mapasService);
        Services.AddSingleton(_pessoaisService);
    }

    [Fact]
    public void Render_ExibeTituloGrafico()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("GRÁFICO NUMEROLÓGICO"));
    }

    [Fact]
    public void Render_ExibeNomeUtilizado()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("JOSE DA SILVA"));
    }

    [Fact]
    public void Render_ExibeNumeroMotivacao()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        // Grade fake: O(7)+E(5)=12 vogais → "12 => 3"
        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='motivacao']").TextContent.Should().Be("12 => 3"));
    }

    [Fact]
    public void Render_ExibeNumeroImpressao()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        // Grade fake: J(1)+S(3)=4 consoantes → "4 => 4"
        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='impressao']").TextContent.Should().Be("4 => 4"));
    }

    [Fact]
    public void Render_ExibeNumeroExpressao()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        // Grade fake: total 12+4=16 → "16 => 7"
        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='expressao']").TextContent.Should().Be("16 => 7"));
    }

    [Fact]
    public void Render_ExibeNumeroDestino()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='destino']").TextContent.Should().Be("9"));
    }

    [Fact]
    public void Render_ExibeAnoPessoalDinamico()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='ano-pessoal']").TextContent.Should().Be("5"));
    }

    [Fact]
    public void Render_ExibeGradeLetras()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.FindAll("[data-testid='celula-letra']").Should().HaveCount(4));
    }

    [Fact]
    public void Render_ExibeCiclosDeVida()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
        {
            cut.Find("[data-testid='ciclo1']").TextContent.Should().Be("3");
            cut.Find("[data-testid='ciclo2']").TextContent.Should().Be("1");
            cut.Find("[data-testid='ciclo3']").TextContent.Should().Be("5");
        });
    }

    [Fact]
    public void Render_ExibeDesafios()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
        {
            cut.Find("[data-testid='desafio1']").TextContent.Should().Be("2");
            cut.Find("[data-testid='desafio2']").TextContent.Should().Be("4");
            cut.Find("[data-testid='desafio-principal']").TextContent.Should().Be("2");
        });
    }

    [Fact]
    public void Render_ExibeHarmoniaConjugal()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='harmonia-vibra-com']").TextContent.Should().Be("1"));
    }

    [Fact]
    public void Render_ExibeNumeroAmor()
    {
        // Número do Amor = Missão (Expressão + Destino reduzido) — pág. 205
        // _mapaFake.Missao = 7
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='numero-amor']").TextContent.Should().Be("7"));
    }

    [Fact]
    public void Render_ExibeCoresFavoraveis()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Verde"));
    }

    // ── F2.7 — Anos dos Ciclos de Vida ───────────────────────────────────────

    [Fact]
    public void Render_ExibePeriodoCiclo1()
    {
        // DataNascimento: 1985-03-10, FimCiclo1Idade: 27 → de 1985 até 2012
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='ciclo1-periodo']").TextContent.Should().Be("de 1985 até 2012"));
    }

    [Fact]
    public void Render_ExibePeriodoCiclo2()
    {
        // FimCiclo2Idade: 54 → de 2012 até 2039
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='ciclo2-periodo']").TextContent.Should().Be("de 2012 até 2039"));
    }

    [Fact]
    public void Render_ExibePeriodoCiclo3()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='ciclo3-periodo']").TextContent.Should().Be("de 2039 em diante"));
    }

    // ── F2.8 — Anos dos Momentos Decisivos ───────────────────────────────────
    // MD1 dura FimCiclo1Idade (= 27 anos); MD2 e MD3 duram 9 anos cada (livro pág. 196)

    [Fact]
    public void Render_ExibePeriodoMD1()
    {
        // anoFimMD1 = anoFimCiclo1 = 1985 + 27 = 2012
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='md1-periodo']").TextContent.Should().Be("de 1985 até 2012"));
    }

    [Fact]
    public void Render_ExibePeriodoMD2()
    {
        // anoFimMD2 = 2012 + 9 = 2021
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='md2-periodo']").TextContent.Should().Be("de 2012 até 2021"));
    }

    [Fact]
    public void Render_ExibePeriodoMD3()
    {
        // anoFimMD3 = 2021 + 9 = 2030
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='md3-periodo']").TextContent.Should().Be("de 2021 até 2030"));
    }

    [Fact]
    public void Render_ExibePeriodoMD4()
    {
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='md4-periodo']").TextContent.Should().Be("de 2030 em diante"));
    }

    // ── F2.9 — Data de nascimento sem redução (card "Data Natal") ───────────
    // Pág. 135: o dia não se reduz; exibe-se dia, mês e ano brutos.

    [Fact]
    public void Render_ExibeDataNascimentoDia()
    {
        // DataNascimento.Day=10 → exibe "10" (sem redução)
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='data-dia']").TextContent.Should().Be("10"));
    }

    [Fact]
    public void Render_ExibeDataNascimentoMes()
    {
        // DataNascimento.Month=3 → exibe "3" (sem redução, sem zero à esquerda)
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='data-mes']").TextContent.Should().Be("3"));
    }

    [Fact]
    public void Render_ExibeDataNascimentoAno()
    {
        // DataNascimento.Year=1985 → exibe "1985" (sem redução)
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='data-ano']").TextContent.Should().Be("1985"));
    }

    // ── F2.10 — Interpretação da Relação Intervalores ────────────────────────

    [Fact]
    public void Render_ExibeInterpretacaoRelacaoIntervalores()
    {
        // RelacaoIntervalores = 7 (RI-7) → texto do livro contém "análise"
        ConfigurarServicos();
        var cut = RenderGrafico();

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='interp-intervalores']").TextContent
                .Should().Contain("análise"));
    }

    private void ConfigurarServicos()
    {
        _mapasService.ObterAsync(1, 1).Returns(_mapaFake);
        _pessoaisService.ObterAsync(Arg.Any<int>(), Arg.Any<int>()).Returns(_pessoalFake);
    }

    private IRenderedComponent<GraficoNumerologico> RenderGrafico() =>
        RenderComponent<GraficoNumerologico>(p => p
            .Add(c => c.ConsulenteId, 1)
            .Add(c => c.MapaId, 1));
}
