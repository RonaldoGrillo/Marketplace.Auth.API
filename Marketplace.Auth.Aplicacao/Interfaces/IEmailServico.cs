namespace Marketplace.Auth.Aplicacao.Interfaces;

public interface IEmailServico
{
    Task EnviarAsync(string destinatario, string assunto, string corpo, CancellationToken ct = default);
}
