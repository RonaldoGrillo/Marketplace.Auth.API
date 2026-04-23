using System.Net;
using System.Net.Mail;
using Marketplace.Auth.Aplicacao.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Auth.Repositorio.Servicos;

public class EmailServico(IConfiguration configuration) : IEmailServico
{
    public async Task EnviarAsync(string destinatario, string assunto, string corpo, CancellationToken ct = default)
    {
        var smtpConfig = configuration.GetSection("Smtp");

        using var cliente = new SmtpClient(smtpConfig["Host"])
        {
            Port = int.Parse(smtpConfig["Porta"] ?? "587"),
            Credentials = new NetworkCredential(smtpConfig["Usuario"], smtpConfig["Senha"]),
            EnableSsl = bool.Parse(smtpConfig["UsarSsl"] ?? "true")
        };

        using var mensagem = new MailMessage(smtpConfig["Remetente"]!, destinatario, assunto, corpo);
        await cliente.SendMailAsync(mensagem, ct);
    }
}
