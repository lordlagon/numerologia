using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

/// <summary>
/// Testes de referência para o nome "André Luiz Xavier de Macedo".
/// Usados para validar o motor de cálculo contra um mapa conhecido.
///
/// ── Decomposição ─────────────────────────────────────────────────────────
/// ANDRÉ:  A(V,1)  N(C,5)  D(C,4)  R(C,2)  É(V,7)
/// LUIZ:   L(C,3)  U(V,6)  I(V,1)  Z(C,7)
/// XAVIER: X(C,6)  A(V,1)  V(C,6)  I(V,1)  E(V,5)  R(C,2)
/// DE:     D(C,4)  E(V,5)
/// MACEDO: M(C,4)  A(V,1)  C(C,3)  E(V,5)  D(C,4)  O(V,7)
///
/// ── Somas ────────────────────────────────────────────────────────────────
/// Soma vogais:       1+7+6+1+1+1+5+5+1+5+7  = 40  → reduz → 4
/// Soma consoantes:   5+4+2+3+7+6+6+2+4+4+3+4 = 50 → reduz → 5
/// Soma total:        40+50 = 90               → reduz → 9
///
/// ── Fig. A ───────────────────────────────────────────────────────────────
/// 1 → 5  (A,I,A,I,A)
/// 2 → 2  (R,R)
/// 3 → 2  (L,C)
/// 4 → 4  (D,D,M,D)
/// 5 → 4  (N,E,E,E)
/// 6 → 3  (U,X,V)
/// 7 → 3  (É,Z,O)
/// 8 → 0
/// 9 → 0
///
/// Lições Cármicas:       [8, 9]  (ausentes de 1–9)
/// Tendências Ocultas:    [1, 4, 5] (1→5x, 4→4x, 5→4x — todos aparecem mais de 3 vezes)
/// Resposta Subconsciente: 9 − 2 = 7
/// Dívidas Cármicas:      []
/// </summary>
public class CalculoMapaAndreTests
{
    private const string Nome = "André Luiz Xavier de Macedo";
    private readonly CalculoMapa _sut = new();
    private readonly ResultadoMapa _r;

    public CalculoMapaAndreTests()
    {
        _r = _sut.Calcular(Nome);
    }

    // ── Grade de Letras ───────────────────────────────────────────────────────

    [Fact]
    public void Grade_TemVinteSetteEntradas()
    {
        // 23 letras + 4 espaços
        _r.GradeLetras.Should().HaveCount(27);
    }

    [Fact]
    public void Grade_ANDRE_ValoresCorretos()
    {
        // A(V,1) N(C,5) D(C,4) R(C,2) É(V,7)
        _r.GradeLetras[0].Should().Be(new EntradaLetra('A', TipoLetra.Vogal, 1));
        _r.GradeLetras[1].Should().Be(new EntradaLetra('N', TipoLetra.Consoante, 5));
        _r.GradeLetras[2].Should().Be(new EntradaLetra('D', TipoLetra.Consoante, 4));
        _r.GradeLetras[3].Should().Be(new EntradaLetra('R', TipoLetra.Consoante, 2));
        _r.GradeLetras[4].Should().Be(new EntradaLetra('É', TipoLetra.Vogal, 7));
    }

    [Fact]
    public void Grade_LUIZ_ValoresCorretos()
    {
        // [5]=espaço  L(C,3) U(V,6) I(V,1) Z(C,7)
        _r.GradeLetras[5].Should().Be(new EntradaLetra(' ', TipoLetra.Espaco, 0));
        _r.GradeLetras[6].Should().Be(new EntradaLetra('L', TipoLetra.Consoante, 3));
        _r.GradeLetras[7].Should().Be(new EntradaLetra('U', TipoLetra.Vogal, 6));
        _r.GradeLetras[8].Should().Be(new EntradaLetra('I', TipoLetra.Vogal, 1));
        _r.GradeLetras[9].Should().Be(new EntradaLetra('Z', TipoLetra.Consoante, 7));
    }

    [Fact]
    public void Grade_XAVIER_ValoresCorretos()
    {
        // [10]=espaço  X(C,6) A(V,1) V(C,6) I(V,1) E(V,5) R(C,2)
        _r.GradeLetras[10].Should().Be(new EntradaLetra(' ', TipoLetra.Espaco, 0));
        _r.GradeLetras[11].Should().Be(new EntradaLetra('X', TipoLetra.Consoante, 6));
        _r.GradeLetras[12].Should().Be(new EntradaLetra('A', TipoLetra.Vogal, 1));
        _r.GradeLetras[13].Should().Be(new EntradaLetra('V', TipoLetra.Consoante, 6));
        _r.GradeLetras[14].Should().Be(new EntradaLetra('I', TipoLetra.Vogal, 1));
        _r.GradeLetras[15].Should().Be(new EntradaLetra('E', TipoLetra.Vogal, 5));
        _r.GradeLetras[16].Should().Be(new EntradaLetra('R', TipoLetra.Consoante, 2));
    }

    [Fact]
    public void Grade_DE_ValoresCorretos()
    {
        // [17]=espaço  D(C,4) E(V,5)
        _r.GradeLetras[17].Should().Be(new EntradaLetra(' ', TipoLetra.Espaco, 0));
        _r.GradeLetras[18].Should().Be(new EntradaLetra('D', TipoLetra.Consoante, 4));
        _r.GradeLetras[19].Should().Be(new EntradaLetra('E', TipoLetra.Vogal, 5));
    }

    [Fact]
    public void Grade_MACEDO_ValoresCorretos()
    {
        // [20]=espaço  M(C,4) A(V,1) C(C,3) E(V,5) D(C,4) O(V,7)
        _r.GradeLetras[20].Should().Be(new EntradaLetra(' ', TipoLetra.Espaco, 0));
        _r.GradeLetras[21].Should().Be(new EntradaLetra('M', TipoLetra.Consoante, 4));
        _r.GradeLetras[22].Should().Be(new EntradaLetra('A', TipoLetra.Vogal, 1));
        _r.GradeLetras[23].Should().Be(new EntradaLetra('C', TipoLetra.Consoante, 3));
        _r.GradeLetras[24].Should().Be(new EntradaLetra('E', TipoLetra.Vogal, 5));
        _r.GradeLetras[25].Should().Be(new EntradaLetra('D', TipoLetra.Consoante, 4));
        _r.GradeLetras[26].Should().Be(new EntradaLetra('O', TipoLetra.Vogal, 7));
    }

    // ── Motivação, Impressão, Expressão ───────────────────────────────────────

    [Fact]
    public void NumeroMotivacao_DeveSerQuatro()
    {
        // Soma vogais: 1+7+6+1+1+1+5+5+1+5+7 = 40 → 4
        _r.NumeroMotivacao.Should().Be(4);
    }

    [Fact]
    public void NumeroImpressao_DeveSerCinco()
    {
        // Soma consoantes: 5+4+2+3+7+6+6+2+4+4+3+4 = 50 → 5
        _r.NumeroImpressao.Should().Be(5);
    }

    [Fact]
    public void NumeroExpressao_DeveSerNove()
    {
        // Soma total: 90 → 9
        _r.NumeroExpressao.Should().Be(9);
    }

    // ── Dívidas Cármicas ──────────────────────────────────────────────────────

    [Fact]
    public void DividasCarmicas_MotivacaoQuatro_GeraDivida13()
    {
        // Motivação = 4 → Dívida 13 (pág. 117: valor final 4 é indicador de Dívida 13)
        // Expressão = 9 → sem dívida (9 não é 4/5/7/1)
        // Dia e Destino são verificados no GeradorMapa, não no CalculoMapa
        _r.DividasCarmicas.Should().BeEquivalentTo([13]);
    }

    // ── Fig. A ────────────────────────────────────────────────────────────────

    [Fact]
    public void FiguraA_ContagemCorreta()
    {
        _r.FiguraA[1].Should().Be(5); // A, I, A, I, A
        _r.FiguraA[2].Should().Be(2); // R, R
        _r.FiguraA[3].Should().Be(2); // L, C
        _r.FiguraA[4].Should().Be(4); // D, D, M, D
        _r.FiguraA[5].Should().Be(4); // N, E, E, E
        _r.FiguraA[6].Should().Be(3); // U, X, V
        _r.FiguraA[7].Should().Be(3); // É, Z, O
        _r.FiguraA[8].Should().Be(0);
        _r.FiguraA[9].Should().Be(0);
    }

    // ── Lições Cármicas ───────────────────────────────────────────────────────

    [Fact]
    public void LicoesCarmicas_DeveSerOitoeNove()
    {
        // Valores 8 e 9 estão ausentes do nome
        _r.LicoesCarmicas.Should().BeEquivalentTo([8, 9]);
    }

    // ── Tendências Ocultas ────────────────────────────────────────────────────

    [Fact]
    public void TendenciasOcultas_RetornaTodosComMaisDeTresOcorrencias()
    {
        // 1→5x, 4→4x, 5→4x — todos aparecem mais de 3 vezes (pág. 114)
        _r.TendenciasOcultas.Should().BeEquivalentTo([1, 4, 5]);
    }

    // ── Resposta Subconsciente ────────────────────────────────────────────────

    [Fact]
    public void RespostaSubconsciente_DeveSerSete()
    {
        // 9 − 2 lições cármicas = 7
        _r.RespostaSubconsciente.Should().Be(7);
    }

    // ── Relação Intervalores (pág. 203) ───────────────────────────────────────

    [Fact]
    public void RelacaoIntervalores_PrimeiroNomeAndre_DeveSerSete()
    {
        // Primeiro nome: A(1) N(5) D(4) R(2) É(7) → max=7 → RI=7
        _r.RelacaoIntervalores.Should().Be(7);
    }
}
