using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using NSubstitute;
using Numerologia.Web.Components;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Components;

public class UserMenuTests : TestContext
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly PerfilState _perfilState = new();
    private readonly IRenderedComponent<MudPopoverProvider> _popoverProvider;

    public UserMenuTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton(_authService);
        Services.AddSingleton(_perfilState);
        // MudPopoverProvider não tem ChildContent — renderizado separado;
        // itens de menu abertos aparecem no markup do provider
        _popoverProvider = RenderComponent<MudPopoverProvider>();
    }

    [Fact]
    public void Render_QuandoNaoAutenticado_NaoExibeMenu()
    {
        _authService.GetCurrentUserAsync().Returns((UsuarioInfo?)null);

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Trim().Should().BeEmpty());
    }

    [Fact]
    public void Render_QuandoAutenticado_ExibeNomeDoUsuario()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Ana Lima"));
    }

    [Fact]
    public void Render_QuandoAutenticado_ExibeLinkPerfil()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();

        // abre o menu
        cut.WaitForAssertion(() => cut.Find("button.mud-button-root").Should().NotBeNull());
        cut.Find("button.mud-button-root").Click();

        // itens ficam no popover provider após abertura
        _popoverProvider.WaitForAssertion(() =>
        {
            var link = _popoverProvider.FindAll("a")
                .FirstOrDefault(a => a.GetAttribute("href")?.Contains("perfil") == true);
            link.Should().NotBeNull();
        });
    }

    [Fact]
    public void Render_QuandoAutenticado_ExibeBotaoSair()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() => cut.Find("button.mud-button-root").Should().NotBeNull());
        cut.Find("button.mud-button-root").Click();

        _popoverProvider.WaitForAssertion(() =>
            _popoverProvider.Markup.Should().Contain("Sair"));
    }

    [Fact]
    public void Render_ComNomeExibicao_ExibeNomeExibicaoNoBotao()
    {
        _authService.GetCurrentUserAsync()
            .Returns(new UsuarioInfo("Ana Google", "ana@test.com", "Ana Numeróloga"));

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Ana Numeróloga"));
    }

    [Fact]
    public void Render_SemNomeExibicao_ExibeNomeGoogle()
    {
        _authService.GetCurrentUserAsync()
            .Returns(new UsuarioInfo("Ana Google", "ana@test.com", null));

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().Contain("Ana Google"));
    }

    [Fact]
    public void Render_QuandoPerfilStateAtualiza_ExibeNomeAtualizado()
    {
        _authService.GetCurrentUserAsync()
            .Returns(new UsuarioInfo("Ana Google", "ana@test.com", null));

        var cut = RenderComponent<UserMenu>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Ana Google"));

        _perfilState.Atualizar("Ana Numeróloga");

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Ana Numeróloga"));
    }

    [Fact]
    public async Task BotaoSair_QuandoClicado_ChamaLogout()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));
        _authService.LogoutAsync().Returns(Task.CompletedTask);

        var cut = RenderComponent<UserMenu>();

        // abre o menu
        cut.WaitForAssertion(() => cut.Find("button.mud-button-root").Should().NotBeNull());
        cut.Find("button.mud-button-root").Click();

        // aguarda o item "Sair" aparecer no popover e clica
        _popoverProvider.WaitForAssertion(() =>
            _popoverProvider.Markup.Should().Contain("Sair"));

        // pega o elemento mais específico que contém "Sair" (menor TextContent)
        var btnSair = _popoverProvider.FindAll("*")
            .Where(e => e.TextContent.Contains("Sair"))
            .OrderBy(e => e.TextContent.Length)
            .First();
        await cut.InvokeAsync(() => btnSair.Click());

        await _authService.Received(1).LogoutAsync();
    }
}
