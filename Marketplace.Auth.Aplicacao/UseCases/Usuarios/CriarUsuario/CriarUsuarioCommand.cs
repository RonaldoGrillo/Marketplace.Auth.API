using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Dominio.Enums;
using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;

public record CriarUsuarioCommand(
    string Nome,
    string Email,
    string Senha,
    EUsuarioFuncao Funcao = EUsuarioFuncao.Comprador)
    : IRequest<UsuarioDto>;
