namespace Numerologia.Core.Calculos;

public static class TabelaCabalistica
{
    private static readonly Dictionary<char, int> _valoresBase = new()
    {
        ['a'] = 1, ['i'] = 1, ['q'] = 1, ['j'] = 1, ['y'] = 1,
        ['b'] = 2, ['k'] = 2, ['r'] = 2,
        ['c'] = 3, ['g'] = 3, ['l'] = 3, ['s'] = 3,
        ['d'] = 4, ['m'] = 4, ['t'] = 4,
        ['e'] = 5, ['h'] = 5, ['n'] = 5,
        ['u'] = 6, ['v'] = 6, ['w'] = 6, ['x'] = 6, ['ç'] = 6,
        ['o'] = 7, ['z'] = 7,
        ['f'] = 8, ['p'] = 8,
    };

    // Letras base de cada diacrítico (para aplicar a regra)
    private static readonly Dictionary<char, char> _baseDosDiacriticos = new()
    {
        // Agudo
        ['á'] = 'a', ['é'] = 'e', ['í'] = 'i', ['ó'] = 'o', ['ú'] = 'u',
        // Circunflexo
        ['â'] = 'a', ['ê'] = 'e', ['î'] = 'i', ['ô'] = 'o', ['û'] = 'u',
        // Til
        ['ã'] = 'a', ['õ'] = 'o',
        // Grave
        ['à'] = 'a', ['è'] = 'e', ['ì'] = 'i', ['ò'] = 'o', ['ù'] = 'u',
        // Trema
        ['ä'] = 'a', ['ë'] = 'e', ['ï'] = 'i', ['ö'] = 'o', ['ü'] = 'u',
        // Ring (Unicode com precomposição disponível)
        ['å'] = 'a', ['ů'] = 'u', ['ė'] = 'e',
    };

    private static readonly HashSet<char> _agudo      = new() { 'á', 'é', 'í', 'ó', 'ú' };
    private static readonly HashSet<char> _circunflexo = new() { 'â', 'ê', 'î', 'ô', 'û' };
    private static readonly HashSet<char> _til         = new() { 'ã', 'õ' };
    private static readonly HashSet<char> _grave       = new() { 'à', 'è', 'ì', 'ò', 'ù' };
    private static readonly HashSet<char> _trema       = new() { 'ä', 'ë', 'ï', 'ö', 'ü' };
   
    // Ring (bolinha acima): +7, igual ao circunflexo — símbolo sugerido em assinaturas
    // Unicode: å (a+ring), ů (u+ring). Para outras letras sem Unicode composto,
    // usar o método ObterValorComRing(char letraBase).
    private static readonly Dictionary<char, char> _ring = new()
    {
        ['å'] = 'a', // a com ring (U+00E5)
        ['ů'] = 'u', // u com ring (U+016F)
        ['ė'] = 'e', // e com ring (U+0117)
    };

    private static readonly HashSet<char> _vogais = new()
    {
        'a', 'e', 'i', 'o', 'u', 'y',
        'á', 'é', 'í', 'ó', 'ú',
        'â', 'ê', 'î', 'ô', 'û',
        'ã', 'õ',
        'à', 'è', 'ì', 'ò', 'ù',
        'ä', 'ë', 'ï', 'ö', 'ü',
    };

    // Reduz o valor somando os dígitos (ex: 12 → 3, 14 → 5). Valores ≤ 9 não mudam.
    private static int Reduzir(int valor) =>
        valor > 9 ? (valor / 10) + (valor % 10) : valor;

    public static int ObterValor(char letra)
    {
        var c = char.ToLowerInvariant(letra);

        if (_valoresBase.TryGetValue(c, out var valorBase))
            return valorBase;

        if (!_baseDosDiacriticos.TryGetValue(c, out var letraBase))
            return 0;

        var valorLetraBase = _valoresBase[letraBase];

        if (_agudo.Contains(c))        return Reduzir(valorLetraBase + 2);
        if (_circunflexo.Contains(c))  return Reduzir(valorLetraBase + 7);
        if (_til.Contains(c))          return Reduzir(valorLetraBase + 3);
        if (_grave.Contains(c))        return Reduzir(valorLetraBase * 2);
        if (_trema.Contains(c))        return Reduzir(valorLetraBase * 2);
        if (_ring.ContainsKey(c))      return Reduzir(valorLetraBase + 7);

        return 0;
    }

    // Retorna o valor de uma letra simples com ring aplicado (+7), reduzido se > 9.
    // Usar quando a numeróloga indica que a letra tem ring na assinatura
    // e não há caractere Unicode composto disponível (ex: 'i' com ring).
    public static int ObterValorComRing(char letraBase)
    {
        var c = char.ToLowerInvariant(letraBase);
        return _valoresBase.TryGetValue(c, out var valor) ? Reduzir(valor + 7) : 0;
    }

    public static bool EhVogal(char letra) =>
        _vogais.Contains(char.ToLowerInvariant(letra));
}
