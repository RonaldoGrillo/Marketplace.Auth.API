namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresIn,
    Guid UsuarioId,
    string Nome,
    string Email);
