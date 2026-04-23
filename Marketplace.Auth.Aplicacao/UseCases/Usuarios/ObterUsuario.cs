using Mapster;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios;

public record ObterUsuarioQuery(Guid Id) : IRequest<UsuarioDto>;

public sealed class ObterUsuarioQueryHandler(IUsuarioRepositorio repositorio)
    : IRequestHandler<ObterUsuarioQuery, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(ObterUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken)
            ?? throw new UsuarioNaoEncontradoException(request.Id);

        return usuario.Adapt<UsuarioDto>();
    }
}
