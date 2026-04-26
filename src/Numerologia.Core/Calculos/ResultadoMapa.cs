namespace Numerologia.Core.Calculos;

public record ResultadoMapa(
    IReadOnlyList<EntradaLetra> GradeLetras,
    int NumeroMotivacao,
    int NumeroImpressao,
    int NumeroExpressao,
    IReadOnlyList<int> DividasCarmicas,
    IReadOnlyDictionary<int, int> FiguraA,
    IReadOnlyList<int> LicoesCarmicas,
    IReadOnlyList<int> TendenciasOcultas,
    int RespostaSubconsciente,
    int RelacaoIntervalores
);
