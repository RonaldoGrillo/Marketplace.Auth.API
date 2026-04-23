namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiraEm,
    Guid UsuarioId,
    string Nome,
    string Email);
