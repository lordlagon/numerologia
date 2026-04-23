using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;
using Numerologia.Core.Interfaces;
using Numerologia.Infrastructure.Data;

namespace Numerologia.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorGoogleIdAsync(string googleId)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}
