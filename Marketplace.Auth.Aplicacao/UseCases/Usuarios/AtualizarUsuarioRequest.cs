namespace Marketplace.Auth.Aplicacao.UseCases.Usuarios;

public record AtualizarUsuarioRequest(
    Guid Id,
    string Nome,
    string Email,
    string? NomeFantasia,
    DateOnly? DataNascimento,
    string? Telefone);
