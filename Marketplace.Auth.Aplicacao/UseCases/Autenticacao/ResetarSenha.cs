using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao;

public record ResetarSenhaCommand(
    string Email,
    string Token,
    string NovaSenha)
    : IRequest;

public sealed class ResetarSenhaCommandHandler(IUsuarioRepositorio repositorio, ISenhaCriptografiaServico senhaCriptografia)
    : IRequestHandler<ResetarSenhaCommand>
{
    public async Task Handle(ResetarSenhaCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(request.Email);

        if (usuario.TokenRedefinicaoSenha != request.Token ||
            usuario.TokenRedefinicaoSenhaExpiresIn < DateTime.UtcNow)
            throw new DominioException("Token de redefinição inválido ou expirado.");

        var senhaHash = senhaCriptografia.Criptografar(request.NovaSenha);
        usuario.AlterarSenha(senhaHash);
        usuario.LimparTokenRedefinicaoSenha();

        await repositorio.AtualizarAsync(usuario, cancellationToken);
    }
}
