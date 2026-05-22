namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao;

public record ResetarSenhaRequest(
    string Email,
    string Token,
    string NovaSenha);
