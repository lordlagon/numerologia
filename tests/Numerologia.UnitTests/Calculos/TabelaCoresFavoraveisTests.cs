using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class TabelaCoresFavoraveisTests
{
    // Tabela da pág. 220–221 do livro — baseada no Nº de Expressão

    public static TheoryData<int, string[]> DadosCores => new()
    {
        { 1, ["Laranja", "Dourado", "Amarelo", "Branco"] },
        { 2, ["Verde", "Crème", "Branco", "Preto"] },
        { 3, ["Violeta", "Vinho", "Púrpura", "Vermelho"] },
        { 4, ["Azul", "Cinza", "Púrpura", "Ouro"] },
        { 5, ["Rosa", "Tons claros", "Cinza-rosa", "Pêssego"] },
        { 6, ["Rosa", "Musgo", "Azul", "Verde"] },
        { 7, ["Verde", "Amarelo", "Branco", "Cinza", "Azul-claro"] },
        { 8, ["Púrpura", "Cítrus", "Azul", "Preto", "Castanho"] },
        { 9, ["Vermelho", "Rosa", "Coral", "Vinho"] }
    };

    [Theory]
    [MemberData(nameof(DadosCores))]
    public void Consultar_RetornasCoresCorretas(int expressao, string[] esperadas)
    {
        var cores = TabelaCoresFavoraveis.Consultar(expressao);

        cores.Should().Equal(esperadas);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-1)]
    public void Consultar_NumeroForaDaTabela_LancaArgumentOutOfRangeException(int expressao)
    {
        var act = () => TabelaCoresFavoraveis.Consultar(expressao);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
