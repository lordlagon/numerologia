namespace Numerologia.Web.Helpers;

public static class CorFavoravelHelper
{
    private static readonly Dictionary<string, (string Background, bool TextoEscuro)> _mapa = new()
    {
        ["Laranja"]     = ("#fd7e14", false),
        ["Dourado"]     = ("#d4a017", false),
        ["Amarelo"]     = ("#ffc107", false),
        ["Branco"]      = ("#f8f9fa", false),
        ["Verde"]       = ("#198754", true),
        ["Crème"]       = ("#fffdd0", false),
        ["Violeta"]     = ("#7f00ff", true),
        ["Vinho"]       = ("#722f37", true),
        ["Púrpura"]     = ("#6f42c1", true),
        ["Vermelho"]    = ("#dc3545", true),
        ["Azul"]        = ("#0d6efd", true),
        ["Cinza"]       = ("#6c757d", true),
        ["Ouro"]        = ("#ffd700", false),
        ["Rosa"]        = ("#f48fb1", false),
        ["Tons claros"] = ("#e0f7fa", false),
        ["Cinza-rosa"]  = ("#c4aead", false),
        ["Pêssego"]     = ("#ffcba4", false),
        ["Musgo"]       = ("#8a9a5b", true),
        ["Azul-claro"]  = ("#87ceeb", false),
        ["Cítrus"]      = ("#9fb723", false),
        ["Preto"]       = ("#212529", true),
        ["Castanho"]    = ("#6d4c41", true),
        ["Coral"]       = ("#ff7f50", false),
    };

    private const string CinzaPadrao = "#6c757d";

    public static string ObterBackgroundCss(string cor)
        => _mapa.TryGetValue(cor, out var v) ? v.Background : CinzaPadrao;

    public static string ObterTextoCss(string cor)
        => _mapa.TryGetValue(cor, out var v)
            ? (v.TextoEscuro ? "#ffffff" : "#212529")
            : "#ffffff";
}
