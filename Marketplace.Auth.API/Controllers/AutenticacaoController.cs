using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Auth.API.Controllers;

[Route("api/[controller]")]
public class AutenticacaoController(IMediator mediator) : ControladorBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return Ok(resultado);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenDto>> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return Ok(resultado);
    }

    [HttpPost("esqueci-senha")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("resetar-senha")]
    public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return NoContent();
    }
}
