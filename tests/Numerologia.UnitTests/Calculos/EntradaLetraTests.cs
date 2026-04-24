using FluentAssertions;
using Numerologia.Core.Calculos;

namespace Numerologia.UnitTests.Calculos;

public class EntradaLetraTests
{
    private readonly CalculoMapa _sut = new();

    // ANA → A(vogal,1) N(consoante,5) A(vogal,1)
    [Fact]
    public void Calcular_RetornaGradeLetras_ComTodasAsLetras()
    {
        var resultado = _sut.Calcular("ANA");

        resultado.GradeLetras.Should().HaveCount(3);
    }

    [Fact]
    public void Calcular_RetornaGradeLetras_ComTipoVogalCorreto()
    {
        var resultado = _sut.Calcular("ANA");

        resultado.GradeLetras[0].Tipo.Should().Be(TipoLetra.Vogal);
        resultado.GradeLetras[2].Tipo.Should().Be(TipoLetra.Vogal);
    }

    [Fact]
    public void Calcular_RetornaGradeLetras_ComTipoConsonanteCorreto()
    {
        var resultado = _sut.Calcular("ANA");

        resultado.GradeLetras[1].Tipo.Should().Be(TipoLetra.Consoante);
    }

    [Fact]
    public void Calcular_RetornaGradeLetras_ComValoresCabalisticos()
    {
        var resultado = _sut.Calcular("ANA");

        resultado.GradeLetras[0].ValorCabalistico.Should().Be(1); // A
        resultado.GradeLetras[1].ValorCabalistico.Should().Be(5); // N
        resultado.GradeLetras[2].ValorCabalistico.Should().Be(1); // A
    }

    [Fact]
    public void Calcular_RetornaGradeLetras_ComLetraOriginalEmMaiuscula()
    {
        var resultado = _sut.Calcular("ana");

        resultado.GradeLetras[0].Letra.Should().Be('A');
        resultado.GradeLetras[1].Letra.Should().Be('N');
        resultado.GradeLetras[2].Letra.Should().Be('A');
    }

    [Fact]
    public void Calcular_NomeComEspaco_IncluiEspacoNaGrade()
    {
        // "JO AO" → J O [espaço] A O
        var resultado = _sut.Calcular("JO AO");

        resultado.GradeLetras.Should().HaveCount(5);
        resultado.GradeLetras[2].Tipo.Should().Be(TipoLetra.Espaco);
        resultado.GradeLetras[2].ValorCabalistico.Should().Be(0);
    }
}
