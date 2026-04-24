using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Pages.Consulentes;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class ListaConsulentesTests : TestContext
{
    private readonly IConsulentesService _serviceMock;

    public ListaConsulentesTests()
    {
        _serviceMock = Substitute.For<IConsulentesService>();
        Services.AddSingleton(_serviceMock);
    }

    [Fact]
    public void Render_ComConsulentes_MostraNomesNaTabela()
    {
        var consulentes = new List<ConsulenteDto>
        {
            new(1, "Maria Silva",  new DateOnly(1990, 6, 15), null, null, DateTime.UtcNow),
            new(2, "João Santos",  new DateOnly(1985, 3, 20), "joao@test.com", null, DateTime.UtcNow),
        };
        _serviceMock.ListarAsync().Returns(consulentes);

        var cut = RenderComponent<ListaConsulentes>();

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Maria Silva"));
        cut.Markup.Should().Contain("João Santos");
    }

    [Fact]
    public void Render_SemConsulentes_MostraMensagemVazia()
    {
        _serviceMock.ListarAsync().Returns(new List<ConsulenteDto>());

        var cut = RenderComponent<ListaConsulentes>();

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Nenhum consulente"));
    }

    [Fact]
    public void Render_Carregando_NaoMostraTabela()
    {
        var tcs = new TaskCompletionSource<List<ConsulenteDto>>();
        _serviceMock.ListarAsync().Returns(tcs.Task);

        var cut = RenderComponent<ListaConsulentes>();

        cut.Markup.Should().Contain("Carregando");
        cut.Markup.Should().NotContain("<table");
    }

    [Fact]
    public async Task BotaoExcluir_QuandoClicado_ChamamRemoverERemoveDaLista()
    {
        var consulentes = new List<ConsulenteDto>
        {
            new(1, "Para Excluir", new DateOnly(1990, 6, 15), null, null, DateTime.UtcNow),
        };
        _serviceMock.ListarAsync().Returns(consulentes);
        _serviceMock.RemoverAsync(1).Returns(Task.CompletedTask);

        var cut = RenderComponent<ListaConsulentes>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Para Excluir"));

        await cut.Find("[data-testid='excluir-1']").ClickAsync(new());

        await _serviceMock.Received(1).RemoverAsync(1);
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain("Para Excluir"));
    }

    [Fact]
    public void BotaoNovoConsulente_Existe_ComLinkCorreto()
    {
        _serviceMock.ListarAsync().Returns(new List<ConsulenteDto>());

        var cut = RenderComponent<ListaConsulentes>();

        cut.Find("a[href='/consulentes/novo']").Should().NotBeNull();
    }

    [Fact]
    public void IconeMapas_QuandoHaConsulentes_ExibeIconeComLinkCorreto()
    {
        var consulentes = new List<ConsulenteDto>
        {
            new(7, "Ana Teste", new DateOnly(1990, 1, 1), null, null, DateTime.UtcNow),
        };
        _serviceMock.ListarAsync().Returns(consulentes);

        var cut = RenderComponent<ListaConsulentes>();

        cut.WaitForAssertion(() =>
        {
            var link = cut.Find("a[href='/consulentes/7/mapas']");
            link.GetAttribute("aria-label").Should().Be("Mapas");
            link.QuerySelector("i.bi-file-text").Should().NotBeNull();
        });
    }
}
