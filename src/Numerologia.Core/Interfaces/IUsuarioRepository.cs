using Numerologia.Core.Entities;

namespace Numerologia.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorGoogleIdAsync(string googleId);
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
}
