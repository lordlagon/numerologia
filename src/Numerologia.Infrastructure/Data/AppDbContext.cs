using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Numerologia.Core.Calculos;
using Numerologia.Core.Entities;

namespace Numerologia.Infrastructure.Data;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    private static readonly JsonSerializerOptions _jsonOptions = new();

    public DbSet<Usuario>            Usuarios           => Set<Usuario>();
    public DbSet<Consulente>         Consulentes        => Set<Consulente>();
    public DbSet<MapaNumerologico>   Mapas              => Set<MapaNumerologico>();
    public DbSet<DataProtectionKey>  DataProtectionKeys => Set<DataProtectionKey>();

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

        modelBuilder.Entity<Consulente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.NomeCompleto).IsRequired().HasMaxLength(256);
            entity.Property(c => c.Email).HasMaxLength(256);
            entity.Property(c => c.Telefone).HasMaxLength(30);
            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MapaNumerologico>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.NomeUtilizado).IsRequired().HasMaxLength(256);

            entity.HasOne<Consulente>()
                  .WithMany()
                  .HasForeignKey(m => m.ConsulenteId)
                  .OnDelete(DeleteBehavior.Cascade);

            // GradeLetras (EntradaLetra[]) → JSON
            entity.Property(m => m.GradeLetras)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<EntradaLetra[]>(v, _jsonOptions) ?? Array.Empty<EntradaLetra>());

            // Arrays de int → JSON (funciona em SQLite e PostgreSQL)
            entity.Property(m => m.DividasCarmicas)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.LicoesCarmicas)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.TendenciasOcultas)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.DiasMesFavoraveis)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.NumerosHarmonicos)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.HarmoniaAtrai)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.HarmoniaEOpostoA)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.HarmoniaProfundamenteOpostoA)
                .HasConversion(IntArrayConverter());
            entity.Property(m => m.HarmoniaEPassivoEm)
                .HasConversion(IntArrayConverter());

            // string[] → JSON
            entity.Property(m => m.CoresFavoraveis)
                .HasConversion(StringArrayConverter());

            // Dictionary<int,int> → JSON
            entity.Property(m => m.FiguraA)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, _jsonOptions)!);
        });
    }

    private static ValueConverter<int[], string> IntArrayConverter() =>
        new(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<int[]>(v, _jsonOptions) ?? Array.Empty<int>());

    private static ValueConverter<string[], string> StringArrayConverter() =>
        new(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<string[]>(v, _jsonOptions) ?? Array.Empty<string>());
}
