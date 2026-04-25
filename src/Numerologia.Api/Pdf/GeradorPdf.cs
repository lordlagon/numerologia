using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Numerologia.Core.Calculos;
using Numerologia.Core.Entities;
using Numerologia.Core.Services;

namespace Numerologia.Api.Pdf;

/// <summary>
/// Gera o PDF do Mapa Numerológico Cabalístico usando QuestPDF (MIT).
/// Estrutura: Capa → Gráfico (landscape) → Interpretações.
/// </summary>
public static class GeradorPdf
{
    public static byte[] Gerar(MapaNumerologico mapa, string nomeConsulente, string nomeNumerologa)
    {
        var doc = Document.Create(container =>
        {
            AdicionarCapa(container, mapa, nomeConsulente, nomeNumerologa);
            AdicionarGrafico(container, mapa, nomeConsulente);
            AdicionarInterpretacoes(container, mapa, nomeConsulente);
        });

        return doc.GeneratePdf();
    }

    // ── Capa ─────────────────────────────────────────────────────────────────

    private static void AdicionarCapa(IDocumentContainer container, MapaNumerologico mapa,
        string nomeConsulente, string nomeNumerologa)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2.5f, Unit.Centimetre);

            page.Content().Column(col =>
            {
                col.Item().PaddingTop(100).AlignCenter()
                    .Text("MAPA NUMEROLÓGICO CABALÍSTICO")
                    .FontSize(22).Bold();

                col.Item().PaddingTop(50).AlignCenter()
                    .Text(nomeConsulente)
                    .FontSize(16).Bold();

                col.Item().PaddingTop(8).AlignCenter()
                    .Text($"Nascimento: {mapa.DataNascimento:dd/MM/yyyy}")
                    .FontSize(11).FontColor(Colors.Grey.Darken2);

                col.Item().PaddingTop(80).AlignCenter()
                    .Text("Numeróloga")
                    .FontSize(10).FontColor(Colors.Grey.Darken1);

                col.Item().PaddingTop(4).AlignCenter()
                    .Text(nomeNumerologa)
                    .FontSize(13).Bold();

                col.Item().PaddingTop(80).AlignCenter()
                    .Text($"Gerado em {DateTime.Now:dd/MM/yyyy}")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    // ── Gráfico ───────────────────────────────────────────────────────────────

    private static void AdicionarGrafico(IDocumentContainer container, MapaNumerologico mapa,
        string nomeConsulente)
    {
        var anoNasc      = mapa.DataNascimento.Year;
        var anoFimCiclo1 = anoNasc + mapa.FimCiclo1Idade;
        var anoFimCiclo2 = anoNasc + mapa.FimCiclo2Idade;
        var anoFimMD1    = anoFimCiclo1;
        var anoFimMD2    = anoFimMD1 + 9;
        var anoFimMD3    = anoFimMD2 + 9;

        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(1f, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(8));

            page.Header().PaddingBottom(4).BorderBottom(1f, Unit.Point).BorderColor(Colors.Grey.Lighten1)
                .Row(row =>
                {
                    row.RelativeItem().Text("GRÁFICO NUMEROLÓGICO").Bold().FontSize(13);
                    row.RelativeItem().AlignRight()
                        .Text($"{nomeConsulente}  |  {mapa.DataNascimento:dd/MM/yyyy}")
                        .FontSize(9).FontColor(Colors.Grey.Darken2);
                });

            page.Content().PaddingTop(8).Column(col =>
            {
                col.Spacing(5);

                // Grade de letras
                col.Item().Element(c => GradeLetras(c, mapa));

                // Corpo principal: Fig. A | Centro | Direita
                col.Item().Row(row =>
                {
                    row.Spacing(5);
                    row.AutoItem().Element(c => FiguraA(c, mapa));
                    row.RelativeItem().Element(c => ColunaCentral(c, mapa, anoNasc, anoFimCiclo1, anoFimCiclo2, anoFimMD1, anoFimMD2, anoFimMD3));
                    row.ConstantItem(210).Element(c => ColunaDireita(c, mapa, anoNasc, anoFimCiclo1, anoFimCiclo2, anoFimMD1, anoFimMD2, anoFimMD3));
                });
            });
        });
    }

    private static void GradeLetras(IContainer ct, MapaNumerologico mapa)
    {
        ct.Table(tbl =>
        {
            tbl.ColumnsDefinition(c =>
            {
                c.ConstantColumn(52);
                foreach (var _ in mapa.GradeLetras) c.ConstantColumn(13);
                c.RelativeColumn();
                c.ConstantColumn(52);
            });

            void CelulaLabel(string txt) =>
                tbl.Cell().Background(Colors.Grey.Lighten3)
                    .Border(1f, Colors.Grey.Lighten1)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(txt).FontSize(7);

            void CelulaResultado(string txt) =>
                tbl.Cell().Border(1f, Colors.Grey.Medium)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(txt).Bold().FontSize(8);

            // Vogais
            CelulaLabel("Vogais");
            foreach (var e in mapa.GradeLetras)
                tbl.Cell().Border(1f, Colors.Grey.Lighten2)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(e.Tipo == TipoLetra.Vogal ? e.ValorCabalistico.ToString() : "")
                    .Bold().FontSize(7);
            CelulaLabel("Nº de Motivação");
            CelulaResultado($"{mapa.NumeroMotivacao}");

            // Nome
            CelulaLabel("Nome");
            foreach (var e in mapa.GradeLetras)
                tbl.Cell().Border(1f, Colors.Grey.Lighten2)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(e.Tipo != TipoLetra.Espaco ? e.Letra.ToString() : "")
                    .FontSize(7);
            tbl.Cell(); tbl.Cell();

            // Consoante
            CelulaLabel("Consoante");
            foreach (var e in mapa.GradeLetras)
                tbl.Cell().Border(1f, Colors.Grey.Lighten2)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(e.Tipo == TipoLetra.Consoante ? e.ValorCabalistico.ToString() : "")
                    .FontSize(7);
            CelulaLabel("Nº de Impressão");
            CelulaResultado($"{mapa.NumeroImpressao}");

            // Total
            CelulaLabel("TOTAL");
            foreach (var e in mapa.GradeLetras)
                tbl.Cell().Border(1f, Colors.Grey.Lighten2)
                    .AlignCenter().AlignMiddle().Padding(1)
                    .Text(e.Tipo != TipoLetra.Espaco ? e.ValorCabalistico.ToString() : "")
                    .FontSize(7);
            CelulaLabel("Nº de Expressão");
            CelulaResultado($"{mapa.NumeroExpressao}");
        });
    }

    private static void FiguraA(IContainer ct, MapaNumerologico mapa)
    {
        ct.Border(1f, Colors.Grey.Medium).Column(col =>
        {
            col.Item().Background(Colors.Grey.Lighten3).Padding(2)
                .AlignCenter().Text("Fig. \"A\"").Bold().FontSize(7);

            foreach (var kv in mapa.FiguraA.OrderBy(x => x.Key))
                col.Item().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2)
                    .Row(r =>
                    {
                        r.ConstantItem(18).BorderRight(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2)
                            .AlignCenter().AlignMiddle().Padding(1)
                            .Text(kv.Key.ToString()).Bold().FontSize(7);
                        r.ConstantItem(18).AlignCenter().AlignMiddle().Padding(1)
                            .Text(kv.Value.ToString()).FontSize(7);
                    });

            col.Item().AlignCenter().Padding(2)
                .Text("Total de Letras\nem Cada Número")
                .FontSize(6).FontColor(Colors.Grey.Darken1);
        });
    }

    private static void ColunaCentral(IContainer ct, MapaNumerologico mapa,
        int anoNasc, int anoFimCiclo1, int anoFimCiclo2, int anoFimMD1, int anoFimMD2, int anoFimMD3)
    {
        ct.Column(col =>
        {
            col.Spacing(4);

            // Fig. B, C, D em linha
            col.Item().Row(r =>
            {
                r.Spacing(4);

                // Fig. B
                r.RelativeItem().Border(1f, Colors.Grey.Medium).Column(b =>
                {
                    b.Item().Background(Colors.Grey.Lighten3).Padding(2)
                        .AlignCenter().Text("Fig. B — Lições Cármicas").Bold().FontSize(7);
                    b.Item().Padding(3).Text(
                        mapa.LicoesCarmicas.Length > 0
                            ? string.Join("  ", mapa.LicoesCarmicas)
                            : "—").Bold().FontSize(9);
                });

                // Fig. C
                r.RelativeItem().Border(1f, Colors.Grey.Medium).Column(c =>
                {
                    c.Item().Background(Colors.Grey.Lighten3).Padding(2)
                        .AlignCenter().Text("Fig. C — Tendências Ocultas").Bold().FontSize(7);
                    c.Item().Padding(3).Text(
                        mapa.TendenciasOcultas.Length > 0
                            ? string.Join("  ", mapa.TendenciasOcultas)
                            : "—").Bold().FontSize(9);
                });

                // Fig. D
                r.RelativeItem().Border(1f, Colors.Grey.Medium).Column(d =>
                {
                    d.Item().Background(Colors.Grey.Lighten3).Padding(2)
                        .AlignCenter().Text("Fig. D").Bold().FontSize(7);
                    d.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c2 => { c2.RelativeColumn(); c2.ConstantColumn(50); });

                        void FigDRow(string label, string val)
                        {
                            t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2)
                                .Padding(1).Text(label).FontSize(7);
                            t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2)
                                .AlignCenter().Padding(1).Text(val).Bold().FontSize(7);
                        }

                        FigDRow("Resp. Subconsciente", mapa.RespostaSubconsciente.ToString());
                        FigDRow("Mês", $"{mapa.DataNascimento.Month:00} => {mapa.MesNascimentoReduzido}");
                        FigDRow("Dia", $"{mapa.DataNascimento.Day} => {mapa.DiaNascimentoReduzido}");
                        FigDRow("Ano", $"{mapa.DataNascimento.Year} => {mapa.AnoNascimentoReduzido}");
                    });
                });
            });

            // Fig. E
            col.Item().Border(1f, Colors.Grey.Medium).Column(e =>
            {
                e.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .AlignCenter().Text("Fig. E").Bold().FontSize(7);
                e.Item().Padding(4).Row(r2 =>
                {
                    r2.Spacing(10);
                    NumeroCard(r2.AutoItem(), "Dívidas Cármicas",
                        mapa.DividasCarmicas.Length > 0 ? string.Join(" ", mapa.DividasCarmicas) : "—", 9);
                    NumeroCard(r2.AutoItem(), "Nº Destino", mapa.NumeroDestino.ToString(), 14);
                    NumeroCard(r2.AutoItem(), "Missão", mapa.Missao.ToString(), 14);
                });
            });

            // Dias do mês favoráveis
            col.Item().Border(1f, Colors.Grey.Medium).Column(df =>
            {
                df.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .Text("Dias do Mês Favoráveis").Bold().FontSize(7);
                df.Item().Padding(3).Text(string.Join("  ", mapa.DiasMesFavoraveis)).FontSize(8);
            });

            // Nº Harmônicos + Relação Intervalores
            col.Item().Row(r3 =>
            {
                r3.Spacing(4);
                r3.RelativeItem().Border(1f, Colors.Grey.Medium).Column(nh =>
                {
                    nh.Item().Background(Colors.Grey.Lighten3).Padding(2)
                        .Text("Números Harmônicos").Bold().FontSize(7);
                    nh.Item().Padding(3).Text(string.Join("  ", mapa.NumerosHarmonicos)).FontSize(8);
                });
                r3.AutoItem().Border(1f, Colors.Grey.Medium).Column(ri =>
                {
                    ri.Item().Background(Colors.Grey.Lighten3).Padding(2)
                        .AlignCenter().Text("Relação Intervalores").Bold().FontSize(7);
                    ri.Item().Padding(3).AlignCenter()
                        .Text(mapa.RelacaoIntervalores.ToString()).Bold().FontSize(13);
                });
            });

            // Cores favoráveis
            col.Item().Border(1f, Colors.Grey.Medium).Column(cf =>
            {
                cf.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .Text("Cores Favoráveis").Bold().FontSize(7);
                cf.Item().Padding(3).Text(string.Join("  |  ", mapa.CoresFavoraveis)).FontSize(8);
            });
        });
    }

    private static void ColunaDireita(IContainer ct, MapaNumerologico mapa,
        int anoNasc, int anoFimCiclo1, int anoFimCiclo2, int anoFimMD1, int anoFimMD2, int anoFimMD3)
    {
        ct.Column(col =>
        {
            col.Spacing(4);

            // Fig. F — Ciclos de Vida
            col.Item().Border(1f, Colors.Grey.Medium).Column(f =>
            {
                f.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .AlignCenter().Text("Fig. F — Ciclos de Vida").Bold().FontSize(7);
                f.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => { c.ConstantColumn(38); c.ConstantColumn(16); c.ConstantColumn(112); c.ConstantColumn(34); });

                    void CicloRow(string label, int num, string periodo, string idade)
                    {
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).Padding(1).Text(label).FontSize(7);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(num.ToString()).Bold().FontSize(7);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).Padding(1).Text(periodo).FontSize(6).FontColor(Colors.Grey.Darken1);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(idade).FontSize(6);
                    }

                    CicloRow("1º Ciclo", mapa.CicloVida1, $"{anoNasc}–{anoFimCiclo1}", $"até {mapa.FimCiclo1Idade}a");
                    CicloRow("2º Ciclo", mapa.CicloVida2, $"{anoFimCiclo1}–{anoFimCiclo2}", $"até {mapa.FimCiclo2Idade}a");
                    CicloRow("3º Ciclo", mapa.CicloVida3, $"{anoFimCiclo2}+", "em diante");
                });
            });

            // Fig. H — Desafios
            col.Item().Border(1f, Colors.Grey.Medium).Column(h =>
            {
                h.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .AlignCenter().Text("Fig. H — Desafios").Bold().FontSize(7);
                h.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => { c.ConstantColumn(190); c.ConstantColumn(20); });

                    void DesafioRow(string label, int val)
                    {
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).Padding(1).Text(label).FontSize(7);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(val.ToString()).Bold().FontSize(8);
                    }

                    DesafioRow("Primeiro", mapa.Desafio1);
                    DesafioRow("Segundo",  mapa.Desafio2);
                    DesafioRow("Principal", mapa.DesafioPrincipal);
                });
            });

            // Fig. G — Momentos Decisivos
            col.Item().Border(1f, Colors.Grey.Medium).Column(g =>
            {
                g.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .AlignCenter().Text("Fig. G — Momentos Decisivos").Bold().FontSize(7);
                g.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => { c.ConstantColumn(14); c.ConstantColumn(16); c.ConstantColumn(180); });

                    void MdRow(string ord, int val, string periodo)
                    {
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(ord).FontSize(7);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(val.ToString()).Bold().FontSize(8);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).Padding(1).Text(periodo).FontSize(6).FontColor(Colors.Grey.Darken1);
                    }

                    MdRow("1º", mapa.MomentoDecisivo1, $"{anoNasc}–{anoFimMD1}");
                    MdRow("2º", mapa.MomentoDecisivo2, $"{anoFimMD1}–{anoFimMD2}");
                    MdRow("3º", mapa.MomentoDecisivo3, $"{anoFimMD2}–{anoFimMD3}");
                    MdRow("4º", mapa.MomentoDecisivo4, $"{anoFimMD3}+");
                });
            });

            // Harmonia Conjugal
            col.Item().Border(1f, Colors.Grey.Medium).Column(hc =>
            {
                hc.Item().Background(Colors.Grey.Lighten3).Padding(2)
                    .AlignCenter().Text("Harmonia Conjugal").Bold().FontSize(7);
                hc.Item().Table(t =>
                {
                    t.ColumnsDefinition(c => { c.ConstantColumn(162); c.ConstantColumn(48); });

                    void HcRow(string label, string val)
                    {
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).Padding(1).Text(label).FontSize(7);
                        t.Cell().BorderTop(1f, Unit.Point).BorderColor(Colors.Grey.Lighten2).AlignCenter().Padding(1).Text(val).Bold().FontSize(7);
                    }

                    HcRow("Vibra com", mapa.HarmoniaVibraCom.ToString());
                    HcRow("Atrai", string.Join(", ", mapa.HarmoniaAtrai));
                    HcRow("É oposto a", string.Join(", ", mapa.HarmoniaEOpostoA));
                    HcRow("É passivo em", string.Join(", ", mapa.HarmoniaEPassivoEm));
                });
            });
        });
    }

    // ── Interpretações ────────────────────────────────────────────────────────

    private static void AdicionarInterpretacoes(IDocumentContainer container,
        MapaNumerologico mapa, string nomeConsulente)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2f, Unit.Centimetre);
            page.DefaultTextStyle(t => t.FontSize(9));

            page.Header().PaddingBottom(6).BorderBottom(1f, Unit.Point).BorderColor(Colors.Grey.Lighten1)
                .Row(row =>
                {
                    row.RelativeItem().Text("INTERPRETAÇÕES").Bold().FontSize(14);
                    row.RelativeItem().AlignRight()
                        .Text($"{nomeConsulente}  |  {mapa.DataNascimento:dd/MM/yyyy}")
                        .FontSize(9).FontColor(Colors.Grey.Darken2);
                });

            page.Content().PaddingTop(8).Column(col =>
            {
                col.Spacing(6);

                void Card(string titulo, string texto)
                {
                    if (string.IsNullOrEmpty(texto)) return;
                    col.Item().Border(1f, Colors.Grey.Lighten1).Column(c =>
                    {
                        c.Item().Background(Colors.Grey.Lighten3).Padding(4)
                            .Text(titulo).Bold().FontSize(9);
                        c.Item().Padding(5).Text(texto).FontSize(9);
                    });
                }

                Card($"Nº de Motivação — {mapa.NumeroMotivacao}",
                    InterpretacoesNumerologicas.Motivacao(mapa.NumeroMotivacao));
                Card($"Nº de Impressão — {mapa.NumeroImpressao}",
                    InterpretacoesNumerologicas.Impressao(mapa.NumeroImpressao));
                Card($"Nº de Destino — {mapa.NumeroDestino}",
                    InterpretacoesNumerologicas.Destino(mapa.NumeroDestino));
                Card($"Missão — {mapa.Missao}",
                    InterpretacoesNumerologicas.Missao(mapa.Missao));
                Card($"Dia do Nascimento — {mapa.DiaNascimentoReduzido}",
                    InterpretacoesNumerologicas.DiaNascimento(mapa.DiaNascimentoReduzido));
                Card($"Resposta Subconsciente — {mapa.RespostaSubconsciente}",
                    InterpretacoesNumerologicas.RespostaSubconsciente(mapa.RespostaSubconsciente));
                Card($"Relação Intervalores — {mapa.RelacaoIntervalores}",
                    InterpretacoesNumerologicas.RelacaoIntervalores(mapa.RelacaoIntervalores));

                if (mapa.LicoesCarmicas.Length > 0)
                {
                    col.Item().Border(1f, Colors.Grey.Lighten1).Column(c =>
                    {
                        c.Item().Background(Colors.Grey.Lighten3).Padding(4)
                            .Text("Lições Cármicas").Bold().FontSize(9);
                        foreach (var n in mapa.LicoesCarmicas)
                            c.Item().PaddingHorizontal(5).PaddingVertical(2)
                                .Text($"{n} — {InterpretacoesNumerologicas.LicaoCarmica(n)}").FontSize(9);
                    });
                }

                if (mapa.TendenciasOcultas.Length > 0)
                {
                    col.Item().Border(1f, Colors.Grey.Lighten1).Column(c =>
                    {
                        c.Item().Background(Colors.Grey.Lighten3).Padding(4)
                            .Text("Tendências Ocultas").Bold().FontSize(9);
                        foreach (var n in mapa.TendenciasOcultas)
                            c.Item().PaddingHorizontal(5).PaddingVertical(2)
                                .Text($"{n} — {InterpretacoesNumerologicas.TendenciaOculta(n)}").FontSize(9);
                    });
                }

                if (mapa.DividasCarmicas.Length > 0)
                {
                    col.Item().Border(1f, Colors.Grey.Lighten1).Column(c =>
                    {
                        c.Item().Background(Colors.Grey.Lighten3).Padding(4)
                            .Text("Dívidas Cármicas").Bold().FontSize(9);
                        foreach (var n in mapa.DividasCarmicas)
                            c.Item().PaddingHorizontal(5).PaddingVertical(2)
                                .Text($"{n} — {InterpretacoesNumerologicas.DividaCarmica(n)}").FontSize(9);
                    });
                }
            });
        });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void NumeroCard(IContainer ct, string label, string valor, int fontSize)
    {
        ct.Column(c =>
        {
            c.Item().Text(label).FontSize(6).FontColor(Colors.Grey.Darken1);
            c.Item().Text(valor).Bold().FontSize(fontSize);
        });
    }
}
