using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao;

public record EsqueciSenhaCommand(string Email) : IRequest;

public sealed class EsqueciSenhaCommandHandler(IUsuarioRepositorio repositorio, IEmailServico emailServico)
    : IRequestHandler<EsqueciSenhaCommand>
{
    public async Task Handle(EsqueciSenhaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(request.Email);

        var token = Guid.NewGuid().ToString("N");
        var expiresIn = DateTime.UtcNow.AddHours(2);

        usuario.DefinirTokenRedefinicaoSenha(token, expiresIn);
        await repositorio.AtualizarAsync(usuario, cancellationToken);

        var corpo = $"Use o token abaixo para redefinir sua senha (válido por 2 horas):\n\n{token}";
        await emailServico.EnviarAsync(usuario.Email, "Redefinição de Senha", corpo, cancellationToken);
    }
}
