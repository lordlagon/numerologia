using Bunit;
using FluentAssertions;
using Numerologia.Web.Components;

namespace Numerologia.UnitTests.Components;

public class BadgeCorTests : TestContext
{
    [Fact]
    public void Render_ExibeNomeDaCor()
    {
        var cut = RenderComponent<BadgeCor>(p => p.Add(c => c.Cor, "Verde"));

        cut.Markup.Should().Contain("Verde");
    }

    [Fact]
    public void Render_AplicaBackgroundColorNoEstilo()
    {
        var cut = RenderComponent<BadgeCor>(p => p.Add(c => c.Cor, "Azul"));

        cut.Find("span").GetAttribute("style").Should().Contain("background-color");
    }

    [Fact]
    public void Render_AplicaColorNoEstilo()
    {
        var cut = RenderComponent<BadgeCor>(p => p.Add(c => c.Cor, "Azul"));

        cut.Find("span").GetAttribute("style").Should().Contain("color");
    }

    [Fact]
    public void Render_CorClara_UsaTextoPreto()
    {
        var cut = RenderComponent<BadgeCor>(p => p.Add(c => c.Cor, "Branco"));

        cut.Find("span").GetAttribute("style").Should().Contain("#212529");
    }

    [Fact]
    public void Render_CorEscura_UsaTextoBranco()
    {
        var cut = RenderComponent<BadgeCor>(p => p.Add(c => c.Cor, "Preto"));

        cut.Find("span").GetAttribute("style").Should().Contain("#ffffff");
    }
}
