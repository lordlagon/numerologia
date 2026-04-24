namespace Numerologia.Core.Entities;

public class Consulente
{
    public int      Id               { get; private set; }
    public int      UsuarioId        { get; private set; }
    public string   NomeCompleto     { get; private set; }
    public DateOnly DataNascimento   { get; private set; }
    public string?  Email            { get; private set; }
    public string?  Telefone         { get; private set; }
    public DateTime CriadoEm        { get; private set; }

    public Consulente(int usuarioId, string nomeCompleto, DateOnly dataNascimento,
        string? email = null, string? telefone = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nomeCompleto);

        UsuarioId      = usuarioId;
        NomeCompleto   = nomeCompleto;
        DataNascimento = dataNascimento;
        Email          = email;
        Telefone       = telefone;
        CriadoEm      = DateTime.UtcNow;
    }

    // EF Core
    private Consulente() { NomeCompleto = null!; }

    public void Atualizar(string nomeCompleto, DateOnly dataNascimento, string? email, string? telefone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nomeCompleto);

        NomeCompleto   = nomeCompleto;
        DataNascimento = dataNascimento;
        Email          = email;
        Telefone       = telefone;
    }
}
