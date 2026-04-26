using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using Numerologia.UnitTests;
using Numerologia.Web.Pages.Mapas;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class FormMapaTests : MudBlazorTestBase
{
    private readonly IMapasService _mapasService = Substitute.For<IMapasService>();
    private readonly IConsulentesService _consulentesService = Substitute.For<IConsulentesService>();
    private readonly NavigationManager _nav;

    private static readonly DateOnly _dataNasc = new(1985, 3, 10);
    private static readonly ConsulenteDto _consulenteFake = new(
        Id: 1, NomeCompleto: "José da Silva", DataNascimento: _dataNasc,
        Email: null, Telefone: null, CriadoEm: DateTime.UtcNow);

    public FormMapaTests()
    {
        Services.AddSingleton(_mapasService);
        Services.AddSingleton(_consulentesService);
        _nav = Services.GetRequiredService<NavigationManager>();

        _consulentesService.ObterAsync(1).Returns(_consulenteFake);
    }

    [Fact]
    public void Render_ExibeCampoNomeUtilizado()
    {
        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='nome-utilizado']").Should().NotBeNull());
    }

    [Fact]
    public void Render_ExibeCampoDataNascimento()
    {
        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.Find("[data-testid='data-nascimento']").Should().NotBeNull());
    }

    [Fact]
    public void Render_ExibeBotaoGerar()
    {
        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForAssertion(() =>
            cut.Find("button[type='submit']").TextContent.Should().Contain("Gerar"));
    }

    [Fact]
    public async Task Submit_ComNomeValido_ChamaServico()
    {
        var esperado = new MapaResumoDto(1, "JOSE", _dataNasc, 3, 5, 7, 9, DateTime.UtcNow);
        _mapasService.CriarAsync(1, Arg.Any<string>(), Arg.Any<DateOnly>())
            .Returns(esperado);

        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForState(() => !cut.Markup.Contains("Carregando"));
        cut.Find("[data-testid='nome-utilizado']").Change("JOSE");
        await cut.Find("form").SubmitAsync();

        await _mapasService.Received(1).CriarAsync(1, "JOSE", Arg.Any<DateOnly>());
    }

    [Fact]
    public async Task Submit_ComNomeValido_NavegaParaLista()
    {
        _mapasService.CriarAsync(1, Arg.Any<string>(), Arg.Any<DateOnly>())
            .Returns(new MapaResumoDto(1, "JOSE", _dataNasc, 3, 5, 7, 9, DateTime.UtcNow));

        var cut = RenderComponent<FormMapa>(p => p.Add(c => c.ConsulenteId, 1));

        cut.WaitForState(() => !cut.Markup.Contains("Carregando"));
        cut.Find("[data-testid='nome-utilizado']").Change("JOSE");
        await cut.Find("form").SubmitAsync();

        _nav.Uri.Should().Contain("/consulentes/1/mapas");
    }
}
