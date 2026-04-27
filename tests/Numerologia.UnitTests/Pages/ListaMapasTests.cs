using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using Numerologia.Web.Pages.Mapas;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class ListaMapasTests : TestContext
{
    private readonly IMapasService _mapasService = Substitute.For<IMapasService>();

    public ListaMapasTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton(_mapasService);
    }

    [Fact]
    public void Render_QuandoCarregando_ExibeCarregando()
    {
        var tcs = new TaskCompletionSource<List<MapaResumoDto>>();
        _mapasService.ListarAsync(Arg.Any<int>()).Returns(tcs.Task);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.Markup.Should().Contain("Carregando");
        tcs.SetResult([]); // libera o task para não vazar
    }

    [Fact]
    public async Task Render_QuandoListaVazia_ExibeMensagem()
    {
        _mapasService.ListarAsync(1).Returns(new List<MapaResumoDto>());

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));
        await cut.InvokeAsync(() => Task.CompletedTask);

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Nenhum mapa"));
    }

    [Fact]
    public async Task Render_ComMapas_ExibeListagem()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "JOSE DA SILVA", new DateOnly(1985, 3, 10), 3, 5, 7, 9, DateTime.UtcNow),
            new(2, "JOSE DE SOUZA", new DateOnly(1985, 3, 10), 2, 4, 3, 4, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.FindAll("td[data-testid^='mapa-']").Should().HaveCount(2));
    }

    [Fact]
    public async Task Render_ComMapas_ExibeNomeUtilizado()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "CARLOS ROSA", new DateOnly(1985, 3, 10), 2, 4, 5, 6, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("CARLOS ROSA"));
    }

    [Fact]
    public async Task Render_ComMapas_ExibeDataNascimento()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "ANA LIMA", new DateOnly(1990, 7, 25), 3, 5, 7, 9, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("25/07/1990"));
    }

    [Fact]
    public void Render_ComMapas_ExibeBotaoPiramidesDesabilitado()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "JOSE DA SILVA", new DateOnly(1985, 3, 10), 3, 5, 7, 9, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
        {
            var btn = cut.FindAll("button")
                .First(b => b.TextContent.Contains("Pirâmides"));
            btn.HasAttribute("disabled").Should().BeTrue();
        });
    }

    [Fact]
    public async Task BotaoExcluir_QuandoConfirmado_RemoveMapa()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "JOSE DA SILVA", new DateOnly(1985, 3, 10), 3, 5, 7, 9, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas, new List<MapaResumoDto>());
        _mapasService.RemoverAsync(1, 1).Returns(Task.CompletedTask);
        JSInterop.Setup<bool>("confirm", _ => true).SetResult(true);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));
        cut.WaitForAssertion(() => cut.FindAll("button").Should().NotBeEmpty());

        var btnExcluir = cut.FindAll("button").First(b => b.TextContent.Contains("Excluir"));
        await cut.InvokeAsync(() => btnExcluir.Click());

        await _mapasService.Received(1).RemoverAsync(1, 1);
    }

    [Fact]
    public async Task BotaoExcluir_QuandoCancelado_NaoRemoveMapa()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "JOSE DA SILVA", new DateOnly(1985, 3, 10), 3, 5, 7, 9, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);
        JSInterop.Setup<bool>("confirm", _ => true).SetResult(false);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));
        cut.WaitForAssertion(() => cut.FindAll("button").Should().NotBeEmpty());

        var btnExcluir = cut.FindAll("button").First(b => b.TextContent.Contains("Excluir"));
        await cut.InvokeAsync(() => btnExcluir.Click());

        await _mapasService.DidNotReceive().RemoverAsync(Arg.Any<int>(), Arg.Any<int>());
    }

    [Fact]
    public void Render_ComMapas_ExibeLinkEditar()
    {
        var mapas = new List<MapaResumoDto>
        {
            new(1, "JOSE DA SILVA", new DateOnly(1985, 3, 10), 3, 5, 7, 9, DateTime.UtcNow),
        };
        _mapasService.ListarAsync(1).Returns(mapas);

        var cut = RenderComponent<ListaMapas>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
        {
            var link = cut.FindAll("a").FirstOrDefault(a => a.TextContent.Contains("Editar"));
            link.Should().NotBeNull();
            link!.GetAttribute("href").Should().Contain("/editar");
        });
    }
}
