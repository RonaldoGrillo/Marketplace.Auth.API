using Marketplace.Auth.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Auth.Repositorio.Persistencia.Contexto;

public class AuthDbContexto(DbContextOptions<AuthDbContexto> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContexto).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
