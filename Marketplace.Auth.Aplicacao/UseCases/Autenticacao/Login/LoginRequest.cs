namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public record LoginRequest(
    string Email,
    string Senha);
