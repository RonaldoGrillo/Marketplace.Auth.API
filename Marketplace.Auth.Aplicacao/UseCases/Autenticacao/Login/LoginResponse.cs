namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiresIn,
    Guid UsuarioId,
    string Nome,
    string Email);
