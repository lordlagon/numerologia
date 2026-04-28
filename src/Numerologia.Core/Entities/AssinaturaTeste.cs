namespace Numerologia.Core.Entities;

public class AssinaturaTeste
{
    public int      Id             { get; private set; }
    public int      MapaId         { get; private set; }
    public string   Texto          { get; private set; }
    public int      ArcanoMomento  { get; private set; }
    public bool     Escolhida      { get; private set; }
    public DateTime CriadoEm      { get; private set; }

    private AssinaturaTeste() { Texto = string.Empty; } // EF

    public AssinaturaTeste(int mapaId, string texto, int arcanoMomento)
    {
        MapaId        = mapaId;
        Texto         = texto;
        ArcanoMomento = arcanoMomento;
        Escolhida     = false;
        CriadoEm     = DateTime.UtcNow;
    }

    public void Escolher()   => Escolhida = true;
    public void Desmarcar()  => Escolhida = false;
}
