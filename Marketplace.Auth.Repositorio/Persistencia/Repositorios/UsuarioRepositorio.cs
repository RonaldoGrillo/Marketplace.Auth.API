using Marketplace.Auth.Dominio.Entidades;
using Marketplace.Auth.Dominio.Interfaces;
using Marketplace.Auth.Repositorio.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Auth.Repositorio.Persistencia.Repositorios;

public class UsuarioRepositorio(AuthDbContexto contexto) : IUsuarioRepositorio
{
    public async Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        await contexto.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        await contexto.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default) =>
        await contexto.Usuarios.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        await contexto.Usuarios.AddAsync(usuario, ct);
        await contexto.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Usuario usuario, CancellationToken ct = default)
    {
        contexto.Usuarios.Update(usuario);
        await contexto.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Usuario usuario, CancellationToken ct = default)
    {
        contexto.Usuarios.Remove(usuario);
        await contexto.SaveChangesAsync(ct);
    }

    public async Task<RefreshToken?> ObterRefreshTokenAsync(string token, CancellationToken ct = default) =>
        await contexto.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, ct);

    public async Task AdicionarRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await contexto.RefreshTokens.AddAsync(refreshToken, ct);
        await contexto.SaveChangesAsync(ct);
    }

    public async Task RevogarRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.Revogar();
        contexto.RefreshTokens.Update(refreshToken);
        await contexto.SaveChangesAsync(ct);
    }
}
