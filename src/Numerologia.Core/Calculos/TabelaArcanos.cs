namespace Numerologia.Core.Calculos;

public static class TabelaArcanos
{
    private static readonly Dictionary<int, (string Titulo, string Significado)> _arcanos = new()
    {
        [1]  = ("I — O Mago",
                "Capacidade para falar, estudar e escrever; habilidade, diplomacia, astúcia, vontade, energia e poder. Mostra que o consulente deve aceitar riscos, pois tem mente alerta e grande capacidade persuasiva. Guiado pela razão e pela fé em si mesmo, alcançará grandes realizações se perseverar em seus ideais."),

        [2]  = ("II — A Papisa",
                "Percepção espiritual e benéfica; segredo, mistério, intuição, coisas escondidas e sentimento religioso. Mostra que o consulente deve tornar-se mais receptivo e pensar bem antes de decidir. O lado negativo indica risco de preguiça e excesso de fantasias."),

        [3]  = ("III — A Imperatriz",
                "Abundância e fertilidade; sabedoria, força espiritual, ação, evolução, progresso e iniciativa. Representa um ser compreensivo com domínio sobre si mesmo. Simboliza esplendor e mostra êxito nas empresas se o consulente for reto de pensamentos e ações."),

        [4]  = ("IV — O Imperador",
                "Majestade e poder; apoio, estabilidade, proteção, força de execução e riqueza material. Representa a Lei, a perseverança e a certeza. Mostra que nada resiste a uma vontade firme que tem como alavanca a ciência da verdade e da justiça."),

        [5]  = ("V — O Papa (O Hierofante)",
                "Opinião pública; inspiração, ensino, consciência, generosidade, autoridade moral e doação dos conhecimentos. Quando aparece, significa que a realização das esperanças pode depender de um ser mais poderoso — é necessário conhecê-lo para obter seu apoio."),

        [6]  = ("VI — O Namorado (Os Amantes)",
                "Escolha emocional; atração, beleza, idealismo. É a indecisão do ser humano frente a decisões difíceis, porém inevitáveis. Mostra que o consulente deve ter muito cuidado com suas resoluções — a indecisão é, em tudo, mais funesta do que uma escolha má."),

        [7]  = ("VII — O Carro",
                "Triunfo bem-merecido; providência, auxílio, dominação do Espírito sobre a Natureza. Representa o ser equilibrado e bem-sucedido que foi capaz de decidir corretamente. Encerra o conjunto das primeiras sete cartas do desenvolvimento humano."),

        [8]  = ("VIII — A Justiça",
                "Equidade, retidão, equilíbrio entre o bem e o mal. É uma parada para pensar e analisar a situação. No lado positivo: harmonia, honra, estabilidade, lei e virtude. No lado negativo: complicação, fanatismo, abuso e intolerância."),

        [9]  = ("IX — O Ermitão",
                "Prudência, proteção, sabedoria, meditação e espírito de sacrifício. Sob o peso do saber, o ser necessita de uma mudança ou abertura na vida e de uma busca efetiva de autoconhecimento. Preocupações com o passado podem atrasar o desenvolvimento espiritual."),
    };

    public static (string Titulo, string Significado)? Obter(int arcano) =>
        _arcanos.TryGetValue(arcano, out var a) ? a : null;
}
