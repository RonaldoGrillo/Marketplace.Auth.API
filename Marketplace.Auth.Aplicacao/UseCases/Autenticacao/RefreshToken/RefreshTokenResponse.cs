namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.RefreshToken;

public record RefreshTokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresIn);
