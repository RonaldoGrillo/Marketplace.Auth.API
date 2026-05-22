using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Dominio.Enums;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;

public record CriarUsuarioCommand(
    string Nome,
    string Email,
    string Senha,
    string Documento,
    ETipoPessoa TipoPessoa,
    EUsuarioFuncao Funcao = EUsuarioFuncao.Comprador,
    string? NomeFantasia = null,
    DateOnly? DataNascimento = null,
    string? Telefone = null)
    : IRequest<UsuarioDto>;
