namespace Numerologia.Web.Services;

public class PerfilState
{
    public string? NomeExibicao { get; private set; }

    public event Action? OnChange;

    /// <summary>
    /// Atualiza o nome e notifica os assinantes (ex.: após salvar no Perfil).
    /// </summary>
    public void Atualizar(string? nomeExibicao)
    {
        NomeExibicao = nomeExibicao;
        OnChange?.Invoke();
    }

    /// <summary>
    /// Inicializa o valor sem disparar o evento (ex.: carga inicial via /api/me).
    /// </summary>
    public void SilentInit(string? nomeExibicao)
    {
        NomeExibicao = nomeExibicao;
    }
}
