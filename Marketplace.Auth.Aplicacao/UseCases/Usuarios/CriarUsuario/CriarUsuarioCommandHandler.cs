using Mapster;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Entidades;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;

public sealed class CriarUsuarioCommandHandler(IUsuarioRepositorio repositorio, ISenhaCriptografiaServico senhaCriptografia)
    : IRequestHandler<CriarUsuarioCommand, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (await repositorio.ExisteEmailAsync(request.Email, cancellationToken))
            throw new DominioException($"Já existe um usuário com o e-mail '{request.Email}'.");

        var senhaHash = senhaCriptografia.Criptografar(request.Senha);
        var usuario = Usuario.Criar(request.Nome, request.Email, senhaHash, request.Funcao);

        await repositorio.AdicionarAsync(usuario, cancellationToken);

        return usuario.Adapt<UsuarioDto>();
    }
}
