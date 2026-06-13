using Marketplace.Auth.Aplicacao.Servicos;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.RefreshToken;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Auth.API.Controllers;

[Route("api/[controller]")]
public class AutenticacaoController(AutenticacaoServico servico) : ControladorBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var resultado = await servico.LoginAsync(request, ct);
        return Ok(resultado);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var resultado = await servico.RefreshTokenAsync(request, ct);
        return Ok(resultado);
    }

    [HttpPost("esqueci-senha")]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request, CancellationToken ct)
    {
        await servico.EsqueciSenhaAsync(request, ct);
        return NoContent();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        await servico.LogoutAsync(request, ct);
        return NoContent();
    }

    [HttpPost("resetar-senha")]
    public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaRequest request, CancellationToken ct)
    {
        await servico.ResetarSenhaAsync(request, ct);
        return NoContent();
    }
}
