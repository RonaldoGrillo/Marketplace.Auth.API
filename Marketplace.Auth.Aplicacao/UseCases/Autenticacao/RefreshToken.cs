using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao;

public record RefreshTokenCommand(string Token) : IRequest<TokenDto>;

public sealed class RefreshTokenCommandHandler(IUsuarioRepositorio repositorio, ITokenServico tokenServico)
    : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenExistente = await repositorio.ObterRefreshTokenAsync(request.Token, cancellationToken)
            ?? throw new DominioException("Refresh token inválido ou expirado.");

        if (!tokenExistente.EstaValido())
            throw new DominioException("Refresh token inválido ou expirado.");

        var usuario = await repositorio.ObterPorIdAsync(tokenExistente.UsuarioId, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(tokenExistente.UsuarioId);

        await repositorio.RevogarRefreshTokenAsync(tokenExistente, cancellationToken);

        var novoAccessToken = tokenServico.GerarAccessToken(usuario);
        var novoRefreshToken = tokenServico.GerarRefreshToken(usuario.Id);

        await repositorio.AdicionarRefreshTokenAsync(novoRefreshToken, cancellationToken);

        return new TokenDto(novoAccessToken, novoRefreshToken.Token, novoRefreshToken.ExpiresIn);
    }
}
