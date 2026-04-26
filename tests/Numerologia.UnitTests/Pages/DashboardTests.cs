using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using Numerologia.UnitTests;
using Numerologia.Web.Pages;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class DashboardTests : MudBlazorTestBase
{
    private readonly IDashboardService _dashboardService = Substitute.For<IDashboardService>();

    public DashboardTests()
    {
        Services.AddSingleton(_dashboardService);
    }

    [Fact]
    public void Render_QuandoCarregando_ExibeCarregando()
    {
        var tcs = new TaskCompletionSource<DashboardDto>();
        _dashboardService.ObterAsync().Returns(tcs.Task);

        var cut = RenderComponent<Dashboard>();

        cut.Markup.Should().Contain("Carregando");
        tcs.SetResult(new DashboardDto(0, []));
    }

    [Fact]
    public void Render_ExibeTotalDeConsulentes()
    {
        _dashboardService.ObterAsync().Returns(new DashboardDto(7, []));

        var cut = RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("7"));
    }

    [Fact]
    public void Render_ComUltimosMapas_ExibeNomeDoConsulente()
    {
        var mapas = new List<UltimoMapaDto>
        {
            new(1, 1, "JOSE DA SILVA", "JOSE", DateTime.UtcNow),
        };
        _dashboardService.ObterAsync().Returns(new DashboardDto(1, mapas));

        var cut = RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("JOSE DA SILVA"));
    }

    [Fact]
    public void Render_ComUltimosMapas_ExibeNomeUtilizado()
    {
        var mapas = new List<UltimoMapaDto>
        {
            new(1, 1, "JOSE DA SILVA", "JOSE", DateTime.UtcNow),
        };
        _dashboardService.ObterAsync().Returns(new DashboardDto(1, mapas));

        var cut = RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("JOSE"));
    }

    [Fact]
    public void Render_SemMapas_ExibeMensagem()
    {
        _dashboardService.ObterAsync().Returns(new DashboardDto(0, []));

        var cut = RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Nenhum mapa"));
    }

    [Fact]
    public void Render_ComUltimosMapas_ExibeLinkParaOMapa()
    {
        var mapas = new List<UltimoMapaDto>
        {
            new(42, 5, "ANA LIMA", "ANA", DateTime.UtcNow),
        };
        _dashboardService.ObterAsync().Returns(new DashboardDto(1, mapas));

        var cut = RenderComponent<Dashboard>();

        cut.WaitForAssertion(() =>
        {
            var link = cut.FindAll("a").FirstOrDefault(a =>
                a.GetAttribute("href")?.Contains("/consulentes/5/mapas/42") == true);
            link.Should().NotBeNull();
        });
    }
}
