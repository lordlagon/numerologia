using Numerologia.Core.Calculos;
using Numerologia.Core.Entities;
using Numerologia.Core.Services;

namespace PocMudBlazor.Services;

public static class MockDataService
{
    public static readonly MapaNumerologico MapaAndre;

    public static readonly List<(int Id, string Nome, DateOnly DataNascimento, int TotalMapas)> Consulentes =
    [
        (1, "André Luiz Xavier de Macedo", new DateOnly(1984,  7, 28), 1),
        (2, "Maria Silva Santos",          new DateOnly(1990,  3, 15), 2),
        (3, "João Pedro Oliveira",         new DateOnly(1975, 11, 22), 1),
        (4, "Ana Carolina Ferreira",       new DateOnly(1988,  7,  4), 3),
        (5, "Carlos Roberto Souza",        new DateOnly(1965,  9, 30), 0),
    ];

    static MockDataService()
    {
        MapaAndre = new GeradorMapa().Gerar(1, "André Luiz Xavier de Macedo", new DateOnly(1984, 7, 28));
    }

    public static ResultadoPessoal PessoalAndre()
        => new CalculosPessoais().Calcular(new DateOnly(1984, 7, 28), DateOnly.FromDateTime(DateTime.Today));
}
