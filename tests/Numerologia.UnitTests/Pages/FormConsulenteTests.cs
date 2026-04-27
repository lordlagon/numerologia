using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using Numerologia.Web.Pages.Consulentes;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Pages;

public class FormConsulenteTests : TestContext
{
    private readonly IConsulentesService _serviceMock;

    public FormConsulenteTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        _serviceMock = Substitute.For<IConsulentesService>();
        Services.AddSingleton(_serviceMock);
    }

    [Fact]
    public void Render_NovoConsulente_MostraTituloNovo()
    {
        var cut = RenderComponent<FormConsulente>();

        cut.Markup.Should().Contain("Novo Consulente");
    }

    [Fact]
    public async Task Submit_ComNomeValido_ChamaCriarENavega()
    {
        var criado = new ConsulenteDto(1, "Ana Teste", new DateOnly(1990, 6, 15),
            null, null, DateTime.UtcNow);
        _serviceMock.CriarAsync(Arg.Any<SalvarConsulenteDto>()).Returns(criado);

        var cut = RenderComponent<FormConsulente>();

        cut.Find("[data-testid='nome']").Input("Ana Teste");
        await cut.Find("form").SubmitAsync();

        await _serviceMock.Received(1).CriarAsync(Arg.Is<SalvarConsulenteDto>(
            d => d.NomeCompleto == "Ana Teste"));

        var nav = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.Uri.Should().EndWith("/consulentes");
    }

    [Fact]
    public void Submit_SemNome_NaoChamaCriar()
    {
        var cut = RenderComponent<FormConsulente>();

        cut.Find("form").Submit();

        _serviceMock.DidNotReceive().CriarAsync(Arg.Any<SalvarConsulenteDto>());
    }

    [Fact]
    public void Render_ComId_MostraTituloEditar()
    {
        var consulente = new ConsulenteDto(5, "Carlos Editar", new DateOnly(1980, 1, 1),
            "carlos@test.com", null, DateTime.UtcNow);
        _serviceMock.ObterAsync(5).Returns(consulente);

        var cut = RenderComponent<FormConsulente>(p => p.Add(c => c.Id, 5));

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Editar Consulente"));
    }

    [Fact]
    public async Task Submit_Edicao_ChamaAtualizarENavega()
    {
        var consulente = new ConsulenteDto(5, "Carlos Original", new DateOnly(1980, 1, 1),
            null, null, DateTime.UtcNow);
        var atualizado = consulente with { NomeCompleto = "Carlos Atualizado" };
        _serviceMock.ObterAsync(5).Returns(consulente);
        _serviceMock.AtualizarAsync(5, Arg.Any<SalvarConsulenteDto>()).Returns(atualizado);

        var cut = RenderComponent<FormConsulente>(p => p.Add(c => c.Id, 5));
        cut.WaitForAssertion(() => cut.Find("[data-testid='nome']")
            .GetAttribute("value").Should().Be("Carlos Original"));

        cut.Find("[data-testid='nome']").Input("Carlos Atualizado");
        await cut.Find("form").SubmitAsync();

        await _serviceMock.Received(1).AtualizarAsync(5, Arg.Is<SalvarConsulenteDto>(
            d => d.NomeCompleto == "Carlos Atualizado"));

        var nav = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.Uri.Should().EndWith("/consulentes");
    }
}
