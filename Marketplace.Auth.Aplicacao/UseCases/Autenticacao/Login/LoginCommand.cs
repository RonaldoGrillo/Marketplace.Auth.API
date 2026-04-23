using MediatR;

namespace Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;

public record LoginCommand(
    string Email,
    string Senha)
    : IRequest<LoginResponse>;
