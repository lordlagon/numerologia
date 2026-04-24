using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Pages.Mapas;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class FormMapaTests : TestContext
{
    private readonly IMapasService _mapasService = Substitute.For<IMapasService>();
    private readonly NavigationManager _nav;

    public FormMapaTests()
    {
        Services.AddSingleton(_mapasService);
        _nav = Services.GetRequiredService<NavigationManager>();
    }

    [Fact]
    public void Render_ExibeCampoNomeUtilizado()
    {
        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.Find("[data-testid='nome-utilizado']").Should().NotBeNull();
    }

    [Fact]
    public void Render_ExibeBotaoGerar()
    {
        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.Find("button[type='submit']").TextContent.Should().Contain("Gerar");
    }

    [Fact]
    public async Task Submit_ComNomeValido_ChamaServico()
    {
        var esperado = new MapaResumoDto(1, "JOSE", new DateOnly(1985, 3, 10), 7, 9, DateTime.UtcNow);
        _mapasService.CriarAsync(1, Arg.Any<string>())
            .Returns(esperado);

        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.Find("[data-testid='nome-utilizado']").Change("JOSE");
        await cut.Find("form").SubmitAsync();

        await _mapasService.Received(1).CriarAsync(1, "JOSE");
    }

    [Fact]
    public async Task Submit_ComNomeValido_NavegaParaLista()
    {
        _mapasService.CriarAsync(1, Arg.Any<string>())
            .Returns(new MapaResumoDto(1, "JOSE", new DateOnly(1985, 3, 10), 7, 9, DateTime.UtcNow));

        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.Find("[data-testid='nome-utilizado']").Change("JOSE");
        await cut.Find("form").SubmitAsync();

        _nav.Uri.Should().Contain("/consulentes/1/mapas");
    }
}
