using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Marketplace.Auth.Aplicacao.Interfaces;
using Marketplace.Auth.Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Auth.Repositorio.Servicos;

public class TokenServico(IConfiguration configuration) : ITokenServico
{
    public string GerarAccessToken(Usuario usuario)
    {
        var jwtConfig = configuration.GetSection("Jwt");
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Chave"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Funcao.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Emissor"],
            audience: jwtConfig["Audiencia"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtConfig["ExpiracaoMinutos"]!)),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GerarRefreshToken(Guid usuarioId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiraEm = DateTime.UtcNow.AddDays(
            int.Parse(configuration["Jwt:RefreshTokenExpiracaoDias"] ?? "7"));

        return RefreshToken.Criar(token, usuarioId, expiraEm);
    }

    public Guid? ObterUsuarioIdDoToken(string token)
    {
        try
        {
            var jwtConfig = configuration.GetSection("Jwt");
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Chave"]!));
            var handler = new JwtSecurityTokenHandler();

            var parametros = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = chave,
                ValidateIssuer = true,
                ValidIssuer = jwtConfig["Emissor"],
                ValidateAudience = true,
                ValidAudience = jwtConfig["Audiencia"],
                ValidateLifetime = false
            };

            var principal = handler.ValidateToken(token, parametros, out _);
            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}
