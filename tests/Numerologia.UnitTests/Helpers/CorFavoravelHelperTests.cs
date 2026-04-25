using FluentAssertions;
using Numerologia.Web.Helpers;

namespace Numerologia.UnitTests.Helpers;

public class CorFavoravelHelperTests
{
    [Theory]
    [InlineData("Verde")]
    [InlineData("Azul")]
    [InlineData("Vermelho")]
    [InlineData("Preto")]
    [InlineData("Laranja")]
    [InlineData("Branco")]
    [InlineData("Cinza")]
    [InlineData("Rosa")]
    [InlineData("Violeta")]
    [InlineData("Vinho")]
    [InlineData("Púrpura")]
    [InlineData("Dourado")]
    [InlineData("Amarelo")]
    [InlineData("Crème")]
    [InlineData("Ouro")]
    [InlineData("Tons claros")]
    [InlineData("Cinza-rosa")]
    [InlineData("Pêssego")]
    [InlineData("Musgo")]
    [InlineData("Azul-claro")]
    [InlineData("Cítrus")]
    [InlineData("Castanho")]
    [InlineData("Coral")]
    public void ObterCss_CorConhecida_RetornaCorNaoVazia(string cor)
    {
        var css = CorFavoravelHelper.ObterBackgroundCss(cor);
        css.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ObterCss_CorDesconhecida_RetornaCinzaPadrao()
    {
        var css = CorFavoravelHelper.ObterBackgroundCss("Cor Inexistente");
        css.Should().Be("#6c757d");
    }

    [Theory]
    [InlineData("Branco")]
    [InlineData("Amarelo")]
    [InlineData("Crème")]
    [InlineData("Rosa")]
    [InlineData("Pêssego")]
    [InlineData("Tons claros")]
    [InlineData("Azul-claro")]
    [InlineData("Cinza-rosa")]
    [InlineData("Ouro")]
    [InlineData("Dourado")]
    [InlineData("Laranja")]
    [InlineData("Coral")]
    [InlineData("Cítrus")]
    public void ObterTextoCss_CorClara_RetornaPreto(string cor)
    {
        var texto = CorFavoravelHelper.ObterTextoCss(cor);
        texto.Should().Be("#212529");
    }

    [Theory]
    [InlineData("Preto")]
    [InlineData("Azul")]
    [InlineData("Verde")]
    [InlineData("Vinho")]
    [InlineData("Púrpura")]
    [InlineData("Violeta")]
    [InlineData("Cinza")]
    [InlineData("Musgo")]
    [InlineData("Castanho")]
    [InlineData("Vermelho")]
    public void ObterTextoCss_CorEscura_RetornaBranco(string cor)
    {
        var texto = CorFavoravelHelper.ObterTextoCss(cor);
        texto.Should().Be("#ffffff");
    }
}
