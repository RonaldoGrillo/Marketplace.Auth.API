using FluentValidation;
using MediatR;

namespace Marketplace.Auth.Aplicacao.Comportamentos;

public sealed class ComportamentoValidacao<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validadores) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validadores.Any())
            return await next(cancellationToken);

        var contexto = new ValidationContext<TRequest>(request);

        var falhas = validadores
            .Select(v => v.Validate(contexto))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (falhas.Count != 0)
            throw new ValidationException(falhas);

        return await next(cancellationToken);
    }
}
