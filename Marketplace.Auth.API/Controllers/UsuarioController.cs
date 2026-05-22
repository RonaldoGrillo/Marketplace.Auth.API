using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.Servicos;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;
using Marketplace.Auth.Dominio.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Auth.API.Controllers;

[Route("api/[controller]")]
public class UsuarioController(UsuarioServico servico) : ControladorBase
{
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> ObterPorId(Guid id, CancellationToken ct)
    {
        var resultado = await servico.ObterPorIdAsync(id, ct);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Criar([FromBody] CriarUsuarioRequest request, CancellationToken ct)
    {
        var resultado = await servico.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request, CancellationToken ct)
    {
        var resultado = await servico.AtualizarAsync(request with { Id = id }, ct);
        return Ok(resultado);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = nameof(EUsuarioFuncao.Administrador))]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken ct)
    {
        await servico.DeletarAsync(id, ct);
        return NoContent();
    }
}
