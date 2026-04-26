namespace Numerologia.Core.Services;

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

    // ── Dia do Nascimento — dias 1–31, sem redução (pág. 135–175) ──────────
    // Nota: o dia do nascimento é o ÚNICO número que não se reduz.
    // Cada dia tem interpretação própria com pontos positivos e negativos.
    private static readonly Dictionary<int, string> _diaNascimento = new()
    {
        { 1,  "DIA DA LIDERANÇA. Positivos: liderança, criatividade, caráter progressista, vigor, otimismo, fortes convicções, competitividade, independência, sociabilidade. Negativos: arrogância, egoísmo, ciúme, antagonismo, excesso de orgulho, hesitação, impaciência." },
        { 2,  "DIA DA DIPLOMACIA. Positivos: gentileza, tato, boas parcerias, receptividade, intuição, consideração, harmonia. Negativos: desconfiança, falta de objetividade, subserviência, excesso de sensibilidade, emotividade, egoísmo, tendência a ser desonesto." },
        { 3,  "DIA DA POPULARIDADE. Positivos: bem-humorado, feliz, amigável, produtivo, criativo, artístico, amor à liberdade, talento com palavras. Negativos: vaidoso, excesso de imaginação, orgulhoso, extravagante, comodista, preguiçoso, hipócrita, esbanjador." },
        { 4,  "DIA DA PERSISTÊNCIA. Positivos: organização, autodisciplina, firmeza, trabalhador, habilidade, talento com as mãos, pragmático, confiável, preciso. Negativos: falta de comunicação, rigidez, falta de sentimentos, procrastinação, autoritarismo, afeições ocultas, ressentimento, severidade." },
        { 5,  "DIA DA VERSATILIDADE. Positivos: versátil, adaptável, progressista, fortes instintos, magnético, sortudo, ousado, amante da liberdade, perspicaz, curioso, sociável. Negativos: pouco confiável, instável, procrastinador, inconsistente, excessivamente confiante, cabeça-dura." },
        { 6,  "DIA DO AMOR. Positivos: universal, fraterno, compassivo, confiável, compreensivo, solidário, idealista, inclinação doméstica, humanitarista, equilibrado, artístico. Negativos: insatisfeito, ansiedade, timidez, irracionalidade, teimosia, falta de harmonia, dominação, egoísmo, desconfiança, cinismo, egocentrismo." },
        { 7,  "DIA DA INSPIRAÇÃO. Positivos: culto, confiável, meticuloso, idealista, honesto, poderes psíquicos, científico, racional, reflexivo. Negativos: dissimulado, enganoso, pouco amigável, fingido, cético, confuso com detalhes, inoportuno, indiferente, pouco sentimental, sensível às críticas." },
        { 8,  "DIA DO ÊXITO MATERIAL. Positivos: liderança, meticulosidade, trabalhador, autoridade, proteção, poder de cura, bom juízo de valores. Negativos: impaciência, desperdício, intolerância, excesso de trabalho, dominação, desencoraja-se facilmente, falta de planejamento." },
        { 9,  "DIA DO HUMANISMO. Positivos: idealista, humanitário, criativo, sensível, generoso, magnético, poético, caridoso, desapegado, sortudo, popular. Negativos: frustrado, fragmentado, inseguro, egoísta, pouco prático, preocupado." },
        { 10, "DIA DA AUTOCONFIANÇA. Positivos: liderança, criatividade, caráter progressista, vigor, otimismo, fortes convicções, competitividade, independência, autoconfiança. Negativos: arrogância, ciúme, egoísmo, orgulho, antagonismo, falta de controle, hesitação, impaciência." },
        { 11, "DIA DA HARMONIA. Positivos: equilíbrio, concentração, objetividade, entusiasmo, inspiração, espiritualidade, idealismo, intuição, capacidade de curar, humanitarismo, psiquismo. Negativos: complexo de superioridade, desonestidade, falta de objetivos, excesso de emotividade, magoa-se facilmente, egoísmo, falta de clareza, dominador, mesquinho, hipersensível." },
        { 12, "DIA DA AUTO-EXPRESSÃO. Positivos: criativo, atraente, capacidade de iniciativa, disciplinador, assertivo, confiante. Negativos: reclusivo, excêntrico, pouco cooperativo, excessivamente sensível, falta de auto-estima." },
        { 13, "DIA DA PERÍCIA. Positivos: ambicioso, criativo, amor pela liberdade, expressivo, grande iniciativa. Negativos: impulsivo, indeciso, autoritário, pouco emotivo, rebelde." },
        { 14, "DIA DA COMPREENSÃO. Positivos: ações decididas, trabalhador, sortudo, criativo, pragmático, imaginativo, habilidoso. Negativos: excessivamente cauteloso ou impulsivo, instável, sem consideração, teimoso." },
        { 15, "DIA DO MAGNETISMO PESSOAL. Positivos: disposição, generosidade, responsabilidade, amabilidade, cooperação, apreço, idéias criativas. Negativos: inquieto, autocentrado, medo de mudar, perda da fé, preocupação, certa indecisão." },
        { 16, "DIA DO TRIUNFO. Positivos: responsabilidade, integridade, intuição, sociabilidade, cooperação, discernimento. Negativos: preocupação, insatisfação, irritabilidade, egoísmo, ceticismo, falta de solidariedade." },
        { 17, "DIA DA PERSPICÁCIA. Positivos: ponderação, especialização, capacidade de planejamento, senso para negócios, trabalhador, científico, atrai dinheiro. Negativos: distante, teimoso, descuidado, mal-humorado, sensível, crítico, preocupado, desconfiado." },
        { 18, "DIA DO PODER MENTAL. Positivos: progressista, assertivo, alto poder de intuição, corajoso, resoluto, eficiente, capacidade de aconselhar. Negativos: emoções descontroladas, falta de ordem, egoísmo, vaidade, ambição desmedida, incapacidade de concluir trabalhos." },
        { 19, "DIA DO CARÁTER. Positivos: dinamismo, criatividade, liderança, progressismo, otimismo, convicções fortes, competitividade, independência, espírito gregário, muita sorte. Negativos: egocentrismo, preocupação, medo de ser rejeitado, materialismo, impaciência, tendência à depressão." },
        { 20, "DIA DA SENSIBILIDADE. Positivos: boas parcerias, gentileza, tato, receptividade, intuição, consideração, harmonia, presença agradável, embaixador da boa vontade. Negativos: desconfiança, subserviência, timidez, sensibilidade excessiva, egoísmo, tendência a magoar-se." },
        { 21, "DIA DO IDEALISMO. Positivos: inspiração, criatividade, uniões por amor e relacionamentos duradouros. Negativos: dependência, temperamental, nervoso, falta de visão, medo de mudanças." },
        { 22, "DIA DA PRATICIDADE. Positivos: intuição elevada, pragmatismo, praticidade, habilidade com as mãos, capacidade de organização, realismo, resolução de problemas, empreendedor. Negativos: esquemas de enriquecimento rápido, nervosismo, autoritarismo, materialismo, falta de visão, ganância, autopromoção, preguiça." },
        { 23, "DIA DA PERSUASÃO. Positivos: lealdade, responsabilidade, adora viajar, comunicativo, intuitivo, criativo, versátil, paciente, persuasivo, confiável. Negativos: egoísta, inseguro, teimoso, inflexível, crítico, reservado, preconceituoso." },
        { 24, "DIA DA UNIÃO. Positivos: energia, idealismo, habilidades práticas, forte determinação, honestidade, franqueza, justiça, harmonia, diplomacia, alegria, generosidade, amor à casa, ativo, enérgico. Negativos: materialista, muito econômico, crueldade, aversão à rotina, pouco confiável, dominador, teimoso, vingativo, ciumento." },
        { 25, "DIA DO PROGRESSO. Positivos: altamente intuitivo, perfeccionista, perceptivo, mente criativa, ponderado, talento para lidar com as pessoas, capacidade para ganhar dinheiro. Negativos: impulsivo, impaciente, excessivamente emotivo, ciumento, reservado, instável, crítico, mal-humorado." },
        { 26, "DIA DA JUSTIÇA. Positivos: prático, atencioso, orgulhoso da família, entusiástico, corajoso, justo, perseverante. Negativos: teimoso, rebelde, falta de entusiasmo, de persistência, relacionamentos instáveis." },
        { 27, "DIA DA AUDÁCIA. Positivos: versátil, imaginativo, criativo, resoluto, corajoso, compreensivo, inventivo, espiritual, audaz, grande força mental. Negativos: brigão, inquieto, nervoso, desconfiado, protelador." },
        { 28, "DIA DO QUERER. Positivos: compaixão, progressismo, temperamento artístico, ambição, trabalho, vida doméstica estável, voluntarioso. Negativos: sonhador, falta de compaixão, autoritário, agressividade, falta de confiança, orgulho, vive se queixando, excessivamente dependente." },
        { 29, "DIA DA ESPIRITUALIDADE. Positivos: inspiração, equilíbrio, paz interior, generosidade, sucesso, criatividade, intuição, misticismo, capacidade de liderança, mundanismo. Negativos: nervosismo, mau humor, extremismo, falta de consideração, arrogância, orgulho." },
        { 30, "DIA DA REALIZAÇÃO. Positivos: amor à diversão, lealdade, amizade, talento com as palavras, criatividade, generosidade. Negativos: preguiça, obstinação, impaciência, insegurança, indiferença, desperdício de energia, não gosta de ser criticado." },
        { 31, "DIA DA HABILIDADE. Positivos: liderança, criatividade, progressista, vigoroso, otimista, fortes convicções, competitivo, independente, habilidoso. Negativos: arrogância, ciúme, egoísmo, orgulho, fraqueza de caráter, hesitação, impaciência." },
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

    // ── Relação Intervalores — RI (pág. 203) ──────────────────────────────
    // Baseado no valor máximo entre as letras do primeiro nome (nome individual).
    private static readonly Dictionary<int, string> _relacaoIntervalores = new()
    {
        { 1, "Indica independência, ambição e interesses próprios. Também mostra egoísmo e possessividade." },
        { 2, "Indica que a pessoa é dotada de tato e diplomacia, possui grande amor à música e às artes de um modo geral. É harmônico e tem capacidade de cooperação. Por vezes, também indica insegurança e timidez." },
        { 3, "Indica pessoa de rara capacidade de expressão, forte imaginação e senso de humor. Às vezes é sinal de irresponsabilidade e impaciência a uma atitude realista ou materialista." },
        { 4, "Indica que a pessoa é econômica, honesta e tem tendência para o trabalho árduo. Porém, tem carência de concentração e julgamento imparcial; possibilidade de obstinação." },
        { 5, "Indica pessoa impulsiva e nervosa, com grande desejo de sexo. As viagens e as mudanças lhe são altamente favoráveis." },
        { 6, "Indica capacidade de assumir grandes responsabilidades. É de confiança, caseiro, pai e educador nato. Tem tendência a polêmicas, brigas e instabilidade emocional." },
        { 7, "Indica poder de análise, agilidade mental, perfeccionismo, equilíbrio e cultura. Grande inclinação pelos assuntos metafísicos e a se retrair." },
        { 8, "Indica capacidade para negócios, habilidade executiva, liderança, iniciativa, tato e grande senso de valores materiais. Tem tendência a se mostrar como dono da verdade." },
        { 9, "Indica um modo de ver universal. Tem dons artísticos e literários. Adora viajar. Em muitos casos, também indica visão estreita e egocentrismo, ou demasiado desapego e afastamento da realidade." },
    };

    public static string RelacaoIntervalores(int ri) =>
        _relacaoIntervalores.GetValueOrDefault(ri, string.Empty);
}
