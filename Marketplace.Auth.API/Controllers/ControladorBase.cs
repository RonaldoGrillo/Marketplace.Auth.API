using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Auth.API.Controllers;

/// <summary>
/// Classe base para todos os controllers da API.
/// Centraliza [ApiController], [Produces] e demais atributos comuns.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ControladorBase : ControllerBase
{
}
