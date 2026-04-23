using Microsoft.EntityFrameworkCore;
using Numerologia.Core.Entities;

namespace Numerologia.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.GoogleId).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.GoogleId).IsRequired().HasMaxLength(128);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Nome).IsRequired().HasMaxLength(256);
        });
    }
}
