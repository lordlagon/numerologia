namespace Numerologia.Web.Services;

public class PerfilState
{
    public string? NomeExibicao { get; private set; }

    public event Action? OnChange;

    public void Atualizar(string? nomeExibicao)
    {
        NomeExibicao = nomeExibicao;
        OnChange?.Invoke();
    }
}
