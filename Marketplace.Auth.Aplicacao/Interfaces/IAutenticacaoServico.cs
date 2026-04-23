using Marketplace.Auth.Aplicacao.DTOs;

namespace Marketplace.Auth.Aplicacao.Interfaces;

public interface IAutenticacaoServico
{
    Task<TokenDto> LoginAsync(string email, string senha, CancellationToken ct = default);
    Task<TokenDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
