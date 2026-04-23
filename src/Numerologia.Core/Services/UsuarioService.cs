using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;

namespace Numerologia.Core.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Usuario> ObterOuCriarAsync(string googleId, string email, string nome)
    {
        var existente = await _repository.ObterPorGoogleIdAsync(googleId);
        if (existente is not null)
            return existente;

        var novo = new Usuario(googleId, email, nome);
        await _repository.AdicionarAsync(novo);
        return novo;
    }
}
