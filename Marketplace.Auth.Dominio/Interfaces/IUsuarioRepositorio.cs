using Marketplace.Auth.Dominio.Entidades;

namespace Marketplace.Auth.Dominio.Interfaces;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
    Task AtualizarAsync(Usuario usuario, CancellationToken ct = default);
    Task RemoverAsync(Usuario usuario, CancellationToken ct = default);
    Task<RefreshToken?> ObterRefreshTokenAsync(string token, CancellationToken ct = default);
    Task AdicionarRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task RevogarRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task RevogarTodosRefreshTokensAsync(Guid usuarioId, CancellationToken ct = default);
    Task DeletarRefreshTokensRevogadosAsync(Guid usuarioId, CancellationToken ct = default);
}