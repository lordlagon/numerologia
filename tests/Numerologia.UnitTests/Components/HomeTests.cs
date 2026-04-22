using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Numerologia.Web.Services;
using Numerologia.Web.Pages;

namespace Numerologia.UnitTests.Components;

public class HomeTests : TestContext
{
    [Fact]
    public void Home_QuandoNaoAutenticado_ExibeBotaoLogin()
    {
        // Arrange
        var authService = Substitute.For<IAuthService>();
        authService.GetCurrentUserAsync().Returns((UsuarioInfo?)null);
        Services.AddSingleton(authService);

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        cut.Find("a[href='/auth/login']").Should().NotBeNull();
        cut.Markup.Should().NotContain("Sair");
    }

    [Fact]
    public void Home_QuandoAutenticado_ExibeNomeDoUsuario()
    {
        // Arrange
        var authService = Substitute.For<IAuthService>();
        authService.GetCurrentUserAsync().Returns(new UsuarioInfo("João Silva", "joao@example.com"));
        Services.AddSingleton(authService);

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        cut.Markup.Should().Contain("João Silva");
        cut.Markup.Should().Contain("joao@example.com");
    }

    [Fact]
    public void Home_QuandoAutenticado_ExibeBotaoSair()
    {
        // Arrange
        var authService = Substitute.For<IAuthService>();
        authService.GetCurrentUserAsync().Returns(new UsuarioInfo("João Silva", "joao@example.com"));
        Services.AddSingleton(authService);

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        cut.Find("button[id='btn-logout']").Should().NotBeNull();
        cut.Markup.Should().NotContain("Entrar com Google");
    }
}
