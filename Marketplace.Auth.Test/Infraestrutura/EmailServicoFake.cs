using Marketplace.Auth.Aplicacao.Interfaces;

namespace Marketplace.Auth.Test.Infraestrutura;

/// <summary>
/// Substitui o EmailServico real nos testes de integração.
/// Captura o último e-mail enviado para que os testes possam inspecionar
/// o token de redefinição de senha gerado pelo handler EsqueciSenha.
/// </summary>
public sealed class EmailServicoFake : IEmailServico
{
    public string? UltimoDestinatario { get; private set; }
    public string? UltimoCorpo { get; private set; }

    public Task EnviarAsync(string destinatario, string assunto, string corpo, CancellationToken ct = default)
    {
        UltimoDestinatario = destinatario;
        UltimoCorpo = corpo;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Extrai o token da última linha não vazia do corpo do e-mail,
    /// que é o formato usado pelo EsqueciSenhaCommandHandler.
    /// </summary>
    public string ExtrairToken()
    {
        ArgumentNullException.ThrowIfNull(UltimoCorpo, nameof(UltimoCorpo));
        return UltimoCorpo.Split('\n', StringSplitOptions.RemoveEmptyEntries).Last().Trim();
    }
}
