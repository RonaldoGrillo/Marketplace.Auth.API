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

        if (await repositorio.ExisteDocumentoAsync(request.Documento, cancellationToken))
            throw new DominioException("Já existe um usuário com este documento (CPF/CNPJ).");

        var senhaHash = senhaCriptografia.Criptografar(request.Senha);
        var usuario = Usuario.Criar(
            request.Nome,
            request.Email,
            senhaHash,
            request.Documento,
            request.TipoPessoa,
            request.Funcao,
            request.NomeFantasia,
            request.DataNascimento,
            request.Telefone);

        await repositorio.AdicionarAsync(usuario, cancellationToken);

        return usuario.Adapt<UsuarioDto>();
    }
}
