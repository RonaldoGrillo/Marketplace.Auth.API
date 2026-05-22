using FluentValidation;
using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;
using Marketplace.Auth.Dominio.Entidades;
using Marketplace.Auth.Dominio.Excecoes;
using Marketplace.Auth.Dominio.Interfaces;

namespace Marketplace.Auth.Aplicacao.Servicos;

public class UsuarioServico(
    IUsuarioRepositorio repositorio,
    ISenhaCriptografiaServico senhaCriptografia,
    IValidator<CriarUsuarioRequest> validadorCriar,
    IValidator<AtualizarUsuarioRequest> validadorAtualizar)
{
    /// <summary>Cria um novo usuário após validar e checar unicidade.</summary>
    public async Task<UsuarioDto> CriarAsync(CriarUsuarioRequest request, CancellationToken ct = default)
    {
        await validadorCriar.ValidateAndThrowAsync(request, ct);

        if (await repositorio.ExisteEmailAsync(request.Email, ct))
            throw new DominioException($"Já existe um usuário com o e-mail '{request.Email}'.");

        if (await repositorio.ExisteDocumentoAsync(request.Documento, ct))
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

        await repositorio.AdicionarAsync(usuario, ct);
        return ToDto(usuario);
    }

    /// <summary>Retorna o usuário pelo id ou lança UsuarioNaoEncontradoException.</summary>
    public async Task<UsuarioDto> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await repositorio.ObterPorIdAsync(id, ct)
            ?? throw new UsuarioNaoEncontradoException(id);

        return ToDto(usuario);
    }

    /// <summary>Atualiza nome, e-mail e dados opcionais do usuário.</summary>
    public async Task<UsuarioDto> AtualizarAsync(AtualizarUsuarioRequest request, CancellationToken ct = default)
    {
        await validadorAtualizar.ValidateAndThrowAsync(request, ct);

        var usuario = await repositorio.ObterPorIdAsync(request.Id, ct)
            ?? throw new UsuarioNaoEncontradoException(request.Id);

        if (!usuario.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
            await repositorio.ExisteEmailAsync(request.Email, ct))
            throw new DominioException($"Já existe um usuário com o e-mail '{request.Email}'.");

        usuario.Atualizar(request.Nome, request.Email, request.NomeFantasia, request.DataNascimento, request.Telefone);
        await repositorio.AtualizarAsync(usuario, ct);
        return ToDto(usuario);
    }

    /// <summary>Remove o usuário pelo id.</summary>
    public async Task DeletarAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await repositorio.ObterPorIdAsync(id, ct)
            ?? throw new UsuarioNaoEncontradoException(id);

        await repositorio.RemoverAsync(usuario, ct);
    }

    private static UsuarioDto ToDto(Usuario u) => new(
        u.Id, u.Nome, u.NomeFantasia, u.Email, u.Documento,
        u.DataNascimento, u.Telefone, u.TipoPessoa, u.Funcao, u.Status, u.CriadoEm);
}
