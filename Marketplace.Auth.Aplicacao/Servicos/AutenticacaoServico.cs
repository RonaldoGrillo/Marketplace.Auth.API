using FluentValidation;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;

namespace Marketplace.Auth.Aplicacao.Servicos;

public class AutenticacaoServico(
    IUsuarioRepositorio repositorio,
    ISenhaCriptografiaServico senhaCriptografia,
    ITokenServico tokenServico,
    IEmailServico emailServico,
    IValidator<LoginRequest> validadorLogin)
{
    /// <summary>Autentica o usuário e devolve um par access/refresh token.</summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        await validadorLogin.ValidateAndThrowAsync(request, ct);

        var usuario = await repositorio.ObterPorEmailAsync(request.Email, ct)
            ?? throw new CredenciaisInvalidasException();

        if (!senhaCriptografia.Verificar(request.Senha, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        await repositorio.RevogarTodosRefreshTokensAsync(usuario.Id, ct);

        var accessToken = tokenServico.GerarAccessToken(usuario);
        var refreshToken = tokenServico.GerarRefreshToken(usuario.Id);

        await repositorio.AdicionarRefreshTokenAsync(refreshToken, ct);
        await repositorio.DeletarRefreshTokensRevogadosAsync(usuario.Id, ct);

        return new LoginResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresIn,
            usuario.Id,
            usuario.Nome,
            usuario.Email);
    }

    /// <summary>Gera um novo par de tokens a partir de um refresh token válido.</summary>
    public async Task<TokenDto> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var tokenExistente = await repositorio.ObterRefreshTokenAsync(request.Token, ct)
            ?? throw new DominioException("Refresh token inválido ou expirado.");

        if (!tokenExistente.Valido())
            throw new DominioException("Refresh token inválido ou expirado.");

        var usuario = await repositorio.ObterPorIdAsync(tokenExistente.UsuarioId, ct)
            ?? throw new UsuarioNaoEncontradoException(tokenExistente.UsuarioId);

        await repositorio.RevogarRefreshTokenAsync(tokenExistente, ct);

        var novoAccessToken = tokenServico.GerarAccessToken(usuario);
        var novoRefreshToken = tokenServico.GerarRefreshToken(usuario.Id);

        await repositorio.AdicionarRefreshTokenAsync(novoRefreshToken, ct);
        await repositorio.DeletarRefreshTokensRevogadosAsync(usuario.Id, ct);

        return new TokenDto(novoAccessToken, novoRefreshToken.Token, novoRefreshToken.ExpiresIn);
    }

    /// <summary>Envia e-mail com token de redefinição de senha.</summary>
    public async Task EsqueciSenhaAsync(EsqueciSenhaRequest request, CancellationToken ct = default)
    {
        var usuario = await repositorio.ObterPorEmailAsync(request.Email, ct)
            ?? throw new UsuarioNaoEncontradoException(request.Email);

        var token = Guid.NewGuid().ToString("N");
        usuario.DefinirTokenRedefinicaoSenha(token, DateTime.UtcNow.AddHours(2));
        await repositorio.AtualizarAsync(usuario, ct);

        var corpo = $"Use o token abaixo para redefinir sua senha (válido por 2 horas):\n\n{token}";
        await emailServico.EnviarAsync(usuario.Email, "Redefinição de Senha", corpo, ct);
    }

    /// <summary>Redefine a senha usando o token enviado por e-mail.</summary>
    public async Task ResetarSenhaAsync(ResetarSenhaRequest request, CancellationToken ct = default)
    {
        var usuario = await repositorio.ObterPorEmailAsync(request.Email, ct)
            ?? throw new UsuarioNaoEncontradoException(request.Email);

        if (usuario.TokenRedefinicaoSenha != request.Token ||
            usuario.TokenRedefinicaoSenhaExpiresIn < DateTime.UtcNow)
            throw new DominioException("Token de redefinição inválido ou expirado.");

        usuario.AlterarSenha(senhaCriptografia.Criptografar(request.NovaSenha));
        usuario.LimparTokenRedefinicaoSenha();
        await repositorio.AtualizarAsync(usuario, ct);
    }
}
