namespace Numerologia.Core.Calculos;

public enum TipoLetra { Vogal, Consoante, Espaco }

public record EntradaLetra(char Letra, TipoLetra Tipo, int ValorCabalistico);
