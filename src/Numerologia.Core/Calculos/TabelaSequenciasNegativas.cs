namespace Numerologia.Core.Calculos;

public static class TabelaSequenciasNegativas
{
    private static readonly Dictionary<int, string> _significados = new()
    {
        [1] = "Com a iniciativa e a determinação: a pessoa fica limitada, perdendo a coragem de se aventurar em algo novo. Pode, também, ficar um longo período inativo, desempregado ou mesmo impotente para realizar o que quer que seja, permanecendo nesse estado o tempo que durar o Arcano que domina o período. Tal sequência pode ainda provocar doenças cardíacas de vários tipos.",
        [2] = "Com a autoconfiança: esta configuração provoca timidez e indecisão, podendo levar o seu possuidor a ser subjugado por aqueles mais próximos, sejam eles amigos, sócios, colega de trabalho ou simplesmente conhecidos. Faz a pessoa perder a autoestima, limitando-se quanto a seus projetos e realizações. Pode, ainda, causar doenças que provocam dependência química.",
        [3] = "Com a comunicação: a pessoa torna-se incompreendida, falta diálogo sobre tudo e com todos, principalmente com colegas de trabalho e com o/a companheiro/a. Tem dificuldade de se impor em seus projetos e é difícil convencer as pessoas. Esta sequência pode ainda causar doenças respiratórias ou de articulações.",
        [4] = "Com o trabalho: torna difícil qualquer realização profissional. Normalmente é mal remunerado e as perspectivas profissional são difíceis ou tem dificuldade em se manter no emprego ou se dar bem em qualquer atividade. Pode, ainda, causar doenças reumáticas e arteriais.",
        [5] = "Com a instabilidade financeira e pessoal: pode causar mudanças não desejada de casa, de emprego, meio social, de país e de parceria. Sob esta influência, a pessoa tem muitos \"altos e baixos\", não se fixando profissionalmente, sempre à procura de melhores oportunidades, sem, contudo, as encontrar. Pode, também, causar \"fuga\" do meio social em que habita e contrair doenças superficiais e destrutiva, ou seja, doenças de pele.",
        [6] = "Com os afetos e sentimentos: causa decepções com amigos, sócios, colegas de trabalhos, parentes e até com o cônjuge (namorado ou companheiro), que não o/a compreende em seus propósito e sentimento. Em vista dessas decepções, a pessoa pode desenvolver algum tipo de doença cardíaca.",
        [7] = "Com o medo e a intolerância: faz com que a pessoa se afaste de tudo e de todos; pode levar ao desmando, transformando as pessoas em seres dependentes, vaidosos, arrogantes e consequentemente, vítima da própria intolerância. Provoca sentimento de solidão, desânimo, doenças nervosas, dependentes e, em vista disso, possivelmente algum tipo de câncer.",
        [8] = "Com problemas emocionais e também financeiros: esta sequência torna a pessoa arredia, afastando-a das atividades sociais. Caso não seja evoluído espiritualmente, pode se descontrolar emocionalmente com muita facilidade. Sob esta vibração, a pessoa pode oscilar entre a riqueza e a pobreza e, como consequência, pode contrair graves doenças ou mesmo ter algum de seus dependentes com tais sintomas.",
        [9] = "Com relação a finanças: a pessoa passa por sérios problemas financeiros, tem perda de bens (móveis e imóveis), as empresas fracassam e passa por vários tipos de provações provocadas por longos períodos de paralização. Tudo isto pode afetar o sistema nervoso e o coração.",
    };

    public static string? ObterSignificado(int digito) =>
        _significados.TryGetValue(digito, out var s) ? s : null;
}
