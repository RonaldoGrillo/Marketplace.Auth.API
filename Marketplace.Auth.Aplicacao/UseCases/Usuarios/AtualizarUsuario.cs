using Mapster;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios;

public record AtualizarUsuarioCommand(
    Guid Id,
    string Nome,
    string Email)
    : IRequest<UsuarioDto>;

public sealed class AtualizarUsuarioCommandHandler(IUsuarioRepositorio repositorio)
    : IRequestHandler<AtualizarUsuarioCommand, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(request.Id);

        if (!usuario.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
            await repositorio.ExisteEmailAsync(request.Email, cancellationToken))
            throw new DominioException($"Já existe um usuário com o e-mail '{request.Email}'.");

        usuario.Atualizar(request.Nome, request.Email);
        await repositorio.AtualizarAsync(usuario, cancellationToken);

        return usuario.Adapt<UsuarioDto>();
    }
}
