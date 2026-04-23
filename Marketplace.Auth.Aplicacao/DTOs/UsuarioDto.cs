using Marketplace.Auth.Dominio.Enums;

namespace Marketplace.Auth.Aplicacao.DTOs;

public record UsuarioDto(
    Guid Id,
    string Nome,
    string Email,
    EUsuarioFuncao Funcao,
    EUsuarioStatus Status,
    DateTime CriadoEm);
