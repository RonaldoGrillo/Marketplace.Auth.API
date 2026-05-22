using Marketplace.Auth.Dominio.Enums;

namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;

public record CriarUsuarioRequest(
    string Nome,
    string Email,
    string Senha,
    string Documento,
    ETipoPessoa TipoPessoa,
    EUsuarioFuncao Funcao = EUsuarioFuncao.Comprador,
    string? NomeFantasia = null,
    DateOnly? DataNascimento = null,
    string? Telefone = null);
