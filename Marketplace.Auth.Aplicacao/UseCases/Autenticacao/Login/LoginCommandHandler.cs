using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public sealed class LoginCommandHandler(IUsuarioRepositorio repositorio, ISenhaCriptografiaServico senhaCriptografia, ITokenServico tokenServico)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken)
            ?? throw new CredenciaisInvalidasException();

        if (!senhaCriptografia.Verificar(request.Senha, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        var accessToken = tokenServico.GerarAccessToken(usuario);
        var refreshToken = tokenServico.GerarRefreshToken(usuario.Id);

        await repositorio.AdicionarRefreshTokenAsync(refreshToken, cancellationToken);

        return new LoginResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresIn,
            usuario.Id,
            usuario.Nome,
            usuario.Email);
    }
}
