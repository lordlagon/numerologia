using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using NSubstitute;
using Numerologia.UnitTests;
using Numerologia.Web.Components;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Components;

public class UserMenuTests : MudBlazorTestBase
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    public UserMenuTests()
    {
        Services.AddSingleton(_authService);
    }

    [Fact]
    public void Render_QuandoNaoAutenticado_NaoExibeMenu()
    {
        _authService.GetCurrentUserAsync().Returns((UsuarioInfo?)null);

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().NotContain("mud-menu"));
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
    public async Task Render_QuandoAutenticado_ExibeLinkPerfil()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Ana Lima"));

        // Open the menu via public API. In MudBlazor v9, ChildContent is portal-ed to
        // MudPopoverProvider (a separate render tree in bUnit). Use FindAllComponents to
        // search both the component's tree and the popover provider's tree.
        await cut.InvokeAsync(() => cut.FindComponent<MudMenu>().Instance.OpenMenuAsync(new MouseEventArgs(), false));

        cut.WaitForAssertion(() =>
            FindAllComponents<MudMenuItem>(cut)
               .Should().Contain(mi => mi.Markup.Contains("perfil")));
    }

    [Fact]
    public async Task Render_QuandoAutenticado_ExibeBotaoSair()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Ana Lima"));

        await cut.InvokeAsync(() => cut.FindComponent<MudMenu>().Instance.OpenMenuAsync(new MouseEventArgs(), false));

        cut.WaitForAssertion(() =>
            FindAllComponents<MudMenuItem>(cut)
               .Should().Contain(mi => mi.Markup.Contains("Sair")));
    }

    [Fact]
    public async Task BotaoSair_QuandoClicado_ChamaLogout()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));
        _authService.LogoutAsync().Returns(Task.CompletedTask);

        var cut = RenderComponent<UserMenu>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Ana Lima"));

        await cut.InvokeAsync(() => cut.FindComponent<MudMenu>().Instance.OpenMenuAsync(new MouseEventArgs(), false));
        cut.WaitForAssertion(() =>
            FindAllComponents<MudMenuItem>(cut)
               .Should().Contain(mi => mi.Markup.Contains("Sair")));

        // Invoke OnClick handler of Sair item directly
        var sairItem = FindAllComponents<MudMenuItem>(cut)
            .First(mi => mi.Markup.Contains("Sair"));
        await cut.InvokeAsync(() => sairItem.Instance.OnClick.InvokeAsync(new MouseEventArgs()));

        await _authService.Received(1).LogoutAsync();
    }
}
