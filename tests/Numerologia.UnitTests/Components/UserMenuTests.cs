using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Components;
using Numerologia.Web.Services;

namespace Numerologia.UnitTests.Components;

public class UserMenuTests : TestContext
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly PerfilState _perfilState = new();

    public UserMenuTests()
    {
        Services.AddSingleton(_authService);
        Services.AddSingleton(_perfilState);
    }

    [Fact]
    public void Render_QuandoNaoAutenticado_NaoExibeMenu()
    {
        _authService.GetCurrentUserAsync().Returns((UsuarioInfo?)null);

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.Markup.Should().NotContain("dropdown"));
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

        cut.WaitForAssertion(() =>
        {
            var link = cut.FindAll("a").FirstOrDefault(a =>
                a.GetAttribute("href")?.Contains("perfil") == true);
            link.Should().NotBeNull();
        });
    }

    [Fact]
    public void Render_QuandoAutenticado_ExibeBotaoSair()
    {
        _authService.GetCurrentUserAsync().Returns(new UsuarioInfo("Ana Lima", "ana@test.com"));

        var cut = RenderComponent<UserMenu>();

        cut.WaitForAssertion(() =>
            cut.FindAll("button, a")
               .Any(e => e.TextContent.Contains("Sair"))
               .Should().BeTrue());
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
        cut.WaitForAssertion(() =>
            cut.FindAll("button, a").Any(e => e.TextContent.Contains("Sair")).Should().BeTrue());

        var btnSair = cut.FindAll("button").First(b => b.TextContent.Contains("Sair"));
        await cut.InvokeAsync(() => btnSair.Click());

        await _authService.Received(1).LogoutAsync();
    }
}
