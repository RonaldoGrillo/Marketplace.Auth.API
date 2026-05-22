using Marketplace.Auth.Dominio.Enums;

namespace Marketplace.Auth.Aplicacao.DTOs;

public record UsuarioDto(
    Guid Id,
    string Nome,
    string? NomeFantasia,
    string Email,
    string Documento,
    DateOnly? DataNascimento,
    string? Telefone,
    ETipoPessoa TipoPessoa,
    EUsuarioFuncao Funcao,
    EUsuarioStatus Status,
    DateTime CriadoEm);
