using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Auth.API.Controllers;

[Route("api/[controller]")]
public class UsuarioController(IMediator mediator) : ControladorBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new ObterUsuarioQuery(id), ct);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Criar([FromBody] CriarUsuarioCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> Atualizar(Guid id, [FromBody] AtualizarUsuarioCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command with { Id = id }, ct);
        return Ok(resultado);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeletarUsuarioCommand(id), ct);
        return NoContent();
    }
}
