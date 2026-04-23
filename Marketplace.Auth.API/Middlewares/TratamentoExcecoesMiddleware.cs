using FluentValidation;
using Marketplace.Auth.Dominio.Excecoes;
using System.Net;
using System.Text.Json;

namespace Marketplace.Auth.API.Middlewares;

public class TratamentoExcecoesMiddleware(RequestDelegate next, ILogger<TratamentoExcecoesMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado: {Mensagem}", ex.Message);
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, resposta) = ex switch
        {
            ValidationException e => (
                HttpStatusCode.BadRequest,
                (object)new { erros = e.Errors.Select(f => new { campo = f.PropertyName, mensagem = f.ErrorMessage }) }),

            CredenciaisInvalidasException e => (HttpStatusCode.Unauthorized, (object)new { erro = e.Message }),
            UsuarioNaoEncontradoException e => (HttpStatusCode.NotFound, (object)new { erro = e.Message }),
            DominioException e => (HttpStatusCode.BadRequest, (object)new { erro = e.Message }),
            _ => (HttpStatusCode.InternalServerError, (object)new { erro = "Ocorreu um erro interno no servidor." })
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
    }
}
