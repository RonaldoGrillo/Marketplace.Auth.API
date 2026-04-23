namespace Marketplace.Auth.Aplicacao.DTOs;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiraEm);
