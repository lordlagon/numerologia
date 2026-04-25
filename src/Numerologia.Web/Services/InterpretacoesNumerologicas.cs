namespace Numerologia.Web.Services;

/// <summary>
/// Textos interpretativos por número, extraídos do livro
/// "Numerologia Cabalística: A Última Fronteira" — Carlos Rosa.
/// Cada método retorna string.Empty quando não há interpretação
/// disponível para o número informado.
/// </summary>
public static class InterpretacoesNumerologicas
{
    // ── Motivação (pág. 65) ────────────────────────────────────────────
    private static readonly Dictionary<int, string> _motivacao = new()
    {
        { 1,  "Pioneiro, independente, autoconfiante, criativo, originalidade." },
        { 2,  "Paz, cooperação, parceria, diplomacia, harmonia, amizade." },
        { 3,  "Social, alegre, artístico, comunicativo, jovial, expressivo." },
        { 4,  "Prático, organizado, metódico, trabalhador, estável, confiável." },
        { 5,  "Mudança, aventura, liberdade, versatilidade, curiosidade." },
        { 6,  "Harmonia, responsabilidade, amor, família, cuidado, beleza." },
        { 7,  "Introspecção, análise, espiritualidade, meditação, profundidade." },
        { 8,  "Poder, ambição, materialidade, autoridade, eficiência, liderança executiva." },
        { 9,  "Universalidade, humanitarismo, compaixão, idealismo elevado." },
        { 11, "Visão superior, missão espiritual, sensibilidade elevada, inspiração." },
        { 22, "Grandes realizações práticas combinadas com base espiritual." },
    };

    // ── Impressão (pág. 75) ────────────────────────────────────────────
    private static readonly Dictionary<int, string> _impressao = new()
    {
        { 1,  "Líder, executor, determinado, assertivo." },
        { 2,  "Cooperativo, gentil, diplomático, parceiro confiável." },
        { 3,  "Criativo, social, comunicativo, alegre." },
        { 4,  "Organizado, sério, confiável, trabalhador." },
        { 5,  "Dinâmico, versátil, curioso, livre." },
        { 6,  "Harmonioso, responsável, amoroso, cuidador." },
        { 7,  "Reservado, analítico, misterioso, pensador profundo." },
        { 8,  "Poderoso, eficiente, executivo, autoridade." },
        { 9,  "Humanitário, idealista, compassivo, sábio." },
        { 11, "Inspirador, sensitivo, visionário." },
        { 22, "Realizador de grandes obras." },
    };

    // ── Destino — Vibrações (pág. 96–110) ─────────────────────────────
    private static readonly Dictionary<int, string> _destino = new()
    {
        { 1,  "Pioneirismo, liderança e iniciativa. Necessita trabalhar por conta própria ou em cargo de chefia, sendo individualista, íntegro e honesto." },
        { 2,  "Associações e parceria. Tem habilidades diplomáticas naturais; precisa cultivar paciência, cooperação e trabalhar sempre em parceria." },
        { 3,  "Sociabilidade e expressão. Alegre, versátil e talentoso; deve cultivar a criatividade, os contatos sociais e a expressão de suas ideias." },
        { 4,  "Conquista pelo trabalho duro. Dedicado e sólido; precisa desenvolver senso de responsabilidade moral e equilíbrio no poder." },
        { 5,  "Mudança e versatilidade. Viajante, sempre em busca do novo; deve desenvolver análise e seleção, trabalhando com propósito definido." },
        { 6,  "Responsabilidade e harmonia. Dócil, educado, harmonioso; precisa aprender a se ajustar a condições difíceis e a servir com alegria." },
        { 7,  "Sabedoria e interiorização. Busca o entendimento profundo; necessita desenvolver seus poderes mentais, estudar e tornar-se especialista." },
        { 8,  "Poder e realização material. Planejador e organizador; precisa cultivar a eficiência na arte de negociar e compreender as leis da acumulação." },
        { 9,  "Universalidade e humanitarismo. Espirituoso e generoso; necessita servir e aprender a amar o próximo, superando o excesso de sensibilidade." },
        { 11, "Inspiração e missão espiritual. Altamente sensível e intuitivo; precisa investigar o misticismo, confiar na intuição e inspirar os outros pelo exemplo." },
        { 22, "Mestre construtor. Dedicado à humanidade; precisa aprender que justiça, cooperação e serviço prestado também fazem parte do cotidiano." },
    };

    // ── Missão (pág. 122–135) ─────────────────────────────────────────
    private static readonly Dictionary<int, string> _missao = new()
    {
        { 1,  "Liderança. Individualista, inovador e corajoso; deve cultivar habilidades executivas, organização e originalidade. Confiante em seus propósitos e independente." },
        { 2,  "Harmonia. Paz e disposição ordenada é seu lema. Solidificação, capacidade de trabalhar em grupo e servir; é o melhor mediador e diplomata." },
        { 3,  "Criatividade. Sociável, popular, criativo e artístico. A auto-expressão exprime seu número — seja na oratória, escrevendo ou representando. Nasceu para brilhar." },
        { 4,  "Vontade. Com os pés no chão; missão é construir coisas com amor, tolerância, paciência e harmonia. É um lutador que enfrenta obstáculos com valentia e determinação." },
        { 5,  "Versatilidade. Ousado, enérgico e amante da liberdade. Mente investigadora e grande versatilidade mental; deve seguir a própria intuição desligando-se das opiniões alheias." },
        { 6,  "Amor. A família é sua principal fonte de preocupação. O Amor Universal é sua missão invariável; honesto, bondoso, leal e sempre pronto a ajudar quem solicita." },
        { 7,  "Sabedoria. A sabedoria é sua palavra de ordem. Enigmático e profundo; é a vibração da perfeição e das qualidades psíquicas. Deve cercar-se de pessoas inteligentes." },
        { 8,  "Justiça. Liderança, meticulosidade, grandes princípios e poder de cura. Organizado e dedicado; deve ser disciplinado para alcançar progresso sólido com justiça e elevado senso moral." },
        { 9,  "Conhecimento. Idealismo, criatividade, generosidade e desapego. Universalista que deve obter o máximo de conhecimentos; o 'irmão mais velho da humanidade'." },
        { 11, "Fé. A grande virtude do 11 é a Fé — em si mesmo, nos seus ideais, pressentimentos e projetos. Potencial para inspirar pessoas com seus ideais; deve concentrar energias positivas." },
        { 22, "Esperança. Altruísta e voltado quase exclusivamente para a humanidade. Universalidade, intuição elevada, pragmatismo e praticidade. Humanitário com visão realista da vida." },
    };

    // ── Lições Cármicas (pág. 110–113) ────────────────────────────────
    private static readonly Dictionary<int, string> _licaoCarmica = new()
    {
        { 1, "Desenvolver independência, iniciativa e autoconfiança. Em vidas passadas foi preguiçoso e sem ambição." },
        { 2, "Desenvolver cooperação, parceria, diplomacia e paciência. Aprender a trabalhar em grupo e ser econômico." },
        { 3, "Desenvolver expressão, criatividade e comunicação. Eliminar a timidez e cultivar a imaginação." },
        { 4, "Desenvolver organização, disciplina e trabalho metódico. Ser constante e levar os projetos até o fim." },
        { 5, "Desenvolver adaptabilidade e abertura para o novo. Enfrentar mudanças com naturalidade." },
        { 6, "Desenvolver responsabilidade, amor familiar e harmonia. Assumir obrigações domésticas com amor." },
        { 7, "Desenvolver introspecção, espiritualidade e análise. Ser culto, espiritualizado e decidido." },
        { 8, "Desenvolver materialidade positiva, autoridade e eficiência. Dar valor ao dinheiro e gerenciar finanças." },
        { 9, "Desenvolver bondade, amor e compaixão pelos semelhantes. Em vidas passadas foi frio e indiferente." },
    };

    // ── Tendências Ocultas (pág. 114) ──────────────────────────────────
    private static readonly Dictionary<int, string> _tendenciaOculta = new()
    {
        { 1, "Desejo de individualidade. Tendência a ser autoritário, dominador, arrogante e egoísta." },
        { 2, "Desejo de associações. Tendência para depender demasiado dos outros (família e amigos) monetária e emocionalmente." },
        { 3, "Desejo de auto-expressão. Tendência à vaidade, impaciência, presunção e dispersar energias em diversões sem preocupação com o amanhã." },
        { 4, "Desejo de trabalho. Tendência a ser perfeccionista (excesso de detalhes), teimoso, intolerante e obstinado." },
        { 5, "Desejo de mudança e liberdade pessoal. Tendência para viver à custa dos outros, abusar de drogas e álcool, ser precipitado e impulsivo." },
        { 6, "Desejo de realização e responsabilidade. Tendência para preocupar-se excessivamente com a família, ser teimoso, perfeccionista e inflexível." },
        { 7, "Desejo de sabedoria e conhecimento. Tendência para fingimento, intriga, alcoolismo; normalmente sente-se incompreendido e rejeitado." },
        { 8, "Desejo do materialismo. Tendência para preocupar-se excessivamente em ganhar dinheiro, obter bens materiais e poder." },
        { 9, "Desejo de conhecimento e amor universal. Tendência para preocupar-se excessivamente com os problemas mundiais em detrimento de si próprio e da família." },
    };

    // ── Resposta Subconsciente (pág. 116) ──────────────────────────────
    // Fórmula: RS = 9 − (quantidade de Lições Cármicas)
    // O livro apresenta valores de 2 a 9.
    private static readonly Dictionary<int, string> _respostaSubconsciente = new()
    {
        { 2, "Arrogante e mentiroso; não respeita regras; quer que tudo gire em torno de si (egocêntrico)." },
        { 3, "Dispersivo e indisciplinado; reage de forma explosiva e destrutiva em situações de crise." },
        { 4, "Vive perdido num labirinto de detalhes; reações fracas, tende a vacilar e atrapalhar os outros." },
        { 5, "Pessoa tensa e nervosa; numa crise age de forma confusa e impulsiva." },
        { 6, "Sentimental; primeira preocupação em crise é com os entes queridos, objetos de estimação e animais." },
        { 7, "Arredio, não gosta de se envolver com problemas alheios; analisa a situação e se retira para dentro de si." },
        { 8, "Eficiente e organizado; numa crise pode-se contar com ele, pois é seguro e digno de confiança." },
        { 9, "Entediado e impessoal; a maioria das coisas tem pouca importância; numa crise é melhor não contar com ele." },
    };

    // ── Dívidas Cármicas (pág. 117) ────────────────────────────────────
    private static readonly Dictionary<int, string> _dividaCarmica = new()
    {
        { 13, "Representa a morte em todas as suas concepções. Desafio: trabalho duro, disciplina, honestidade e administração de bens." },
        { 14, "Trata dos bens materiais. Desafio: moderação, disciplina, estabilidade e desapego — não se apegar a nada nem a ninguém." },
        { 16, "Manifestação do orgulho, autoritarismo, traição e vaidade. Desafio: humildade, serviço, retidão e reconstrução." },
        { 19, "Número do equilíbrio. Desafio: serviço, generosidade, tolerância e repor aquilo que foi tirado dos outros." },
    };

    // ── Dia do Nascimento reduzido (pág. 135) ──────────────────────────
    private static readonly Dictionary<int, string> _diaNascimento = new()
    {
        { 1,  "Liderança, criatividade, independência." },
        { 2,  "Diplomacia, cooperação, sensibilidade." },
        { 3,  "Expressão, alegria, arte, comunicação." },
        { 4,  "Organização, trabalho, estabilidade." },
        { 5,  "Liberdade, mudança, versatilidade." },
        { 6,  "Responsabilidade, amor, harmonia." },
        { 7,  "Introspecção, espiritualidade, análise." },
        { 8,  "Poder, negócios, autoridade." },
        { 9,  "Humanitarismo, compaixão, missão." },
        { 11, "Inspiração, missão espiritual elevada." },
        { 22, "Construtor de grandes obras." },
    };

    // ── Ano Pessoal (pág. 176) ─────────────────────────────────────────
    private static readonly Dictionary<int, string> _anoPessoal = new()
    {
        { 1, "Novos começos, iniciativas, semear." },
        { 2, "Parceria, paciência, segundo plano." },
        { 3, "Expansão, expressão, criatividade, otimismo." },
        { 4, "Trabalho, organização, consolidação, disciplina." },
        { 5, "Mudanças, liberdade, adaptação." },
        { 6, "Família, responsabilidade, amor, ajuste." },
        { 7, "Introspecção, estudo, análise, espiritualidade." },
        { 8, "Colheita, poder, finanças, realizações materiais." },
        { 9, "Conclusões, encerramento de ciclos, doação." },
    };

    // ── Métodos públicos ───────────────────────────────────────────────
    public static string Motivacao(int numero)             => _motivacao.GetValueOrDefault(numero, string.Empty);
    public static string Impressao(int numero)             => _impressao.GetValueOrDefault(numero, string.Empty);
    public static string Destino(int numero)               => _destino.GetValueOrDefault(numero, string.Empty);
    public static string Missao(int numero)                => _missao.GetValueOrDefault(numero, string.Empty);
    public static string LicaoCarmica(int numero)          => _licaoCarmica.GetValueOrDefault(numero, string.Empty);
    public static string TendenciaOculta(int numero)       => _tendenciaOculta.GetValueOrDefault(numero, string.Empty);
    public static string RespostaSubconsciente(int numero) => _respostaSubconsciente.GetValueOrDefault(numero, string.Empty);
    public static string DividaCarmica(int numero)         => _dividaCarmica.GetValueOrDefault(numero, string.Empty);
    public static string DiaNascimento(int numero)         => _diaNascimento.GetValueOrDefault(numero, string.Empty);
    public static string AnoPessoal(int numero)            => _anoPessoal.GetValueOrDefault(numero, string.Empty);

    // ── Relação Intervalores (pág. 203) ────────────────────────────────
    /// <summary>
    /// Interpreta o Grau de Ascensão = Impressão − Motivação.
    /// Positivo: a pessoa projeta mais ao mundo do que sente por dentro (evolução).
    /// Negativo: revela internamente mais do que projeta para fora.
    /// Zero: equilíbrio entre o que sente e o que projeta.
    /// </summary>
    public static string RelacaoIntervalores(int diferenca)
    {
        if (diferenca > 0)
            return $"Grau de Ascensão positivo (+{diferenca}): a pessoa apresenta ao mundo mais do que sente por dentro — sinal de evolução espiritual. Quanto maior a diferença, maior o desenvolvimento.";
        if (diferenca < 0)
            return $"Grau de Ascensão negativo ({diferenca}): a pessoa revela internamente mais do que projeta para fora.";
        return "Grau de Ascensão neutro (0): equilíbrio entre o que sente internamente e o que projeta ao mundo.";
    }
}
