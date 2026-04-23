using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios;

public record DeletarUsuarioCommand(Guid Id) : IRequest;

public sealed class DeletarUsuarioCommandHandler(IUsuarioRepositorio repositorio)
    : IRequestHandler<DeletarUsuarioCommand>
{
    public async Task Handle(DeletarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(request.Id);

        await repositorio.RemoverAsync(usuario, cancellationToken);
    }
}
