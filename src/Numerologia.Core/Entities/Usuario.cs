namespace Numerologia.Core.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string GoogleId { get; private set; }
    public string Email { get; private set; }
    public string Nome { get; private set; }
    public string? NomeExibicao { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public Usuario(string googleId, string email, string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(googleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        GoogleId = googleId;
        Email = email;
        Nome = nome;
        CriadoEm = DateTime.UtcNow;
    }

    public void AtualizarPerfil(string? nomeExibicao)
    {
        if (nomeExibicao is not null && string.IsNullOrWhiteSpace(nomeExibicao))
            throw new ArgumentException("NomeExibicao não pode ser vazio ou apenas espaços.", nameof(nomeExibicao));

        NomeExibicao = nomeExibicao;
    }

    // EF Core requer construtor sem parâmetros
    private Usuario()
    {
        GoogleId = null!;
        Email = null!;
        Nome = null!;
    }
}
