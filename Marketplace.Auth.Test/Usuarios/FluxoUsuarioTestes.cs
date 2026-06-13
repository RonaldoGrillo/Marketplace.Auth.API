using Marketplace.Auth.Aplicacao.DTOs;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.Login;
using Marketplace.Auth.Aplicacao.UseCases.Autenticacao.RefreshToken;
using Marketplace.Auth.Aplicacao.UseCases.Usuarios.CriarUsuario;
using Marketplace.Auth.Dominio.Enums;
using Marketplace.Auth.Test.Infraestrutura;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Marketplace.Auth.Test.Usuarios;

public class FluxoUsuarioTestes(MarketplaceAuthFactory factory) : IClassFixture<MarketplaceAuthFactory>
{
    // Cria um cliente novo por teste para evitar estado compartilhado nos headers
    private HttpClient NovoCliente() => factory.CreateClient();

    // CPF válido para testes: gerado com dígitos verificadores corretos
    private static string NovoCpfTeste() => GeradorCpfTeste.Gerar();

    private static CriarUsuarioRequest NovoUsuarioAdministrador() => new(
        Nome: $"Usuário Teste {Guid.NewGuid():N}",
        Email: $"teste_{Guid.NewGuid():N}@marketplace.com",
        Senha: "Senha@123",
        Documento: NovoCpfTeste(),
        TipoPessoa: ETipoPessoa.PessoaFisica,
        Funcao: EUsuarioFuncao.Administrador);

    /// <summary>
    /// Cria um usuário e retorna o login já autenticado junto com o UsuarioDto.
    /// Utilitário de arrange para evitar repetição nos testes.
    /// </summary>
    private async Task<(UsuarioDto Usuario, LoginResponse Login)> CriarEAutenticarAsync(
        HttpClient cliente, CriarUsuarioRequest? request = null)
    {
        request ??= NovoUsuarioAdministrador();
        var respostaCriacao = await cliente.PostAsJsonAsync("/api/usuario", request);
        var usuario = await respostaCriacao.Content.ReadFromJsonAsync<UsuarioDto>();
        var respostaLogin = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var login = await respostaLogin.Content.ReadFromJsonAsync<LoginResponse>();
        return (usuario!, login!);
    }

    // ── Criar usuário ────────────────────────────────────────────────────────

    [Fact]
    public async Task QuandoCriarUsuario_DeveRetornar201ComDadosDoUsuario()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();

        var resposta = await cliente.PostAsJsonAsync("/api/usuario", request);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var usuario = await resposta.Content.ReadFromJsonAsync<UsuarioDto>();
        Assert.NotNull(usuario);
        Assert.Equal(request.Nome, usuario.Nome);
        Assert.Equal(request.Email.ToLower(), usuario.Email.ToLower());
    }

    [Fact]
    public async Task QuandoCriarUsuarioComEmailDuplicado_DeveRetornar400()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);

        var resposta = await cliente.PostAsJsonAsync("/api/usuario", request);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoCriarUsuarioComDadosInvalidos_DeveRetornar400()
    {
        var cliente = NovoCliente();

        var resposta = await cliente.PostAsJsonAsync("/api/usuario",
            new { Nome = "", Email = "nao-e-email", Senha = "fraca" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    // ── Autenticação ─────────────────────────────────────────────────────────

    [Fact]
    public async Task QuandoFazerLogin_DeveRetornarAccessTokenERefreshToken()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var login = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));
    }

    [Fact]
    public async Task QuandoFazerLoginComSenhaErrada_DeveRetornar401()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, Senha = "SenhaErrada@999" });

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoFazerRefreshToken_DeveRetornarNovosTokens()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);
        var respostaLogin = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var login = await respostaLogin.Content.ReadFromJsonAsync<LoginResponse>();

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = login!.RefreshToken });

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var token = await resposta.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
    }

    [Fact]
    public async Task QuandoFazerRefreshTokenInvalido_DeveRetornarErro()
    {
        var cliente = NovoCliente();

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = "token-invalido-qualquer" });

        Assert.NotEqual(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoFazerLogout_DeveInvalidarRefreshToken()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);
        var respostaLogin = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var login = await respostaLogin.Content.ReadFromJsonAsync<LoginResponse>();

        var respostaLogout = await cliente.PostAsJsonAsync("/api/autenticacao/logout",
            new { Token = login!.RefreshToken });
        Assert.Equal(HttpStatusCode.NoContent, respostaLogout.StatusCode);

        var respostaRefresh = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = login.RefreshToken });
        Assert.NotEqual(HttpStatusCode.OK, respostaRefresh.StatusCode);
    }

    [Fact]
    public async Task QuandoFazerLogoutComTokenInvalido_DeveRetornar204()
    {
        var cliente = NovoCliente();

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/logout",
            new { Token = "token-que-nao-existe" });

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoDoisLogins_DevemManterSessoesIndependentes()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);

        var respostaLogin1 = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var login1 = await respostaLogin1.Content.ReadFromJsonAsync<LoginResponse>();

        var respostaLogin2 = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var login2 = await respostaLogin2.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotEqual(login1!.RefreshToken, login2!.RefreshToken);

        // Sessão 1 ainda deve funcionar após sessão 2 ser criada
        var respostaRefresh1 = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = login1.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, respostaRefresh1.StatusCode);

        // Logout da sessão 2 não afeta a sessão 1
        await cliente.PostAsJsonAsync("/api/autenticacao/logout", new { Token = login2.RefreshToken });
        var respostaRefresh1AposLogout2 = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = login1.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, respostaRefresh1AposLogout2.StatusCode);
    }

    // ── Redefinição de senha ─────────────────────────────────────────────────

    [Fact]
    public async Task QuandoEsqueciSenha_DeveRetornar204()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/esqueci-senha",
            new { request.Email });

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoResetarSenhaComTokenInvalido_DeveRetornar400()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);
        await cliente.PostAsJsonAsync("/api/autenticacao/esqueci-senha", new { request.Email });

        var resposta = await cliente.PostAsJsonAsync("/api/autenticacao/resetar-senha",
            new { request.Email, Token = "token-errado", NovaSenha = "NovaSenha@456" });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoResetarSenhaComTokenCorreto_DeveBloqueiarSenhaAntigaEPermitirNovaSenha()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", request);
        await cliente.PostAsJsonAsync("/api/autenticacao/esqueci-senha", new { request.Email });
        var token = factory.EmailFake.ExtrairToken();
        const string novaSenha = "NovaSenha@456";

        var respostaReset = await cliente.PostAsJsonAsync("/api/autenticacao/resetar-senha",
            new { request.Email, Token = token, NovaSenha = novaSenha });
        var respostaLoginSenhaAntiga = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        var respostaLoginNovaSenha = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, Senha = novaSenha });

        Assert.Equal(HttpStatusCode.NoContent, respostaReset.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, respostaLoginSenhaAntiga.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respostaLoginNovaSenha.StatusCode);
    }

    // ── Obter usuário ────────────────────────────────────────────────────────

    [Fact]
    public async Task QuandoObterUsuarioSemAutenticacao_DeveRetornar401()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        var respostaCriacao = await cliente.PostAsJsonAsync("/api/usuario", request);
        var usuario = await respostaCriacao.Content.ReadFromJsonAsync<UsuarioDto>();

        var resposta = await cliente.GetAsync($"/api/usuario/{usuario!.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    // ── Atualizar usuário ────────────────────────────────────────────────────

    [Fact]
    public async Task QuandoAtualizarUsuario_DeveRetornarDadosAtualizados()
    {
        var cliente = NovoCliente();
        var request = NovoUsuarioAdministrador();
        var (usuario, login) = await CriarEAutenticarAsync(cliente, request);

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        var novoEmail = $"atualizado_{Guid.NewGuid():N}@marketplace.com";
        var resposta = await cliente.PutAsJsonAsync($"/api/usuario/{usuario.Id}",
            new { Nome = "Nome Atualizado", Email = novoEmail });

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var atualizado = await resposta.Content.ReadFromJsonAsync<UsuarioDto>();
        Assert.Equal("Nome Atualizado", atualizado!.Nome);
        Assert.Equal(novoEmail.ToLower(), atualizado.Email.ToLower());
    }

    [Fact]
    public async Task QuandoAtualizarUsuarioComEmailJaEmUso_DeveRetornar400()
    {
        var cliente = NovoCliente();
        var outraRequest = NovoUsuarioAdministrador();
        await cliente.PostAsJsonAsync("/api/usuario", outraRequest);
        var request = NovoUsuarioAdministrador();
        var (usuario, login) = await CriarEAutenticarAsync(cliente, request);

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        var resposta = await cliente.PutAsJsonAsync($"/api/usuario/{usuario.Id}",
            new { Nome = "Qualquer", Email = outraRequest.Email });

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    // ── Deletar usuário ──────────────────────────────────────────────────────

    [Fact]
    public async Task QuandoDeletarUsuario_DeveRetornar204()
    {
        var cliente = NovoCliente();
        var (usuario, login) = await CriarEAutenticarAsync(cliente);

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        var resposta = await cliente.DeleteAsync($"/api/usuario/{usuario.Id}");

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoDeletarSemAutenticacao_DeveRetornar401()
    {
        var clienteCriacao = NovoCliente();
        var (usuario, _) = await CriarEAutenticarAsync(clienteCriacao);

        var clienteSemAuth = NovoCliente();
        var resposta = await clienteSemAuth.DeleteAsync($"/api/usuario/{usuario.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoNaoAdministradorTentaDeletar_DeveRetornar403()
    {
        var clienteAdmin = NovoCliente();
        var (usuarioAlvo, _) = await CriarEAutenticarAsync(clienteAdmin);

        var compradorRequest = new CriarUsuarioRequest(
            Nome: $"Comprador {Guid.NewGuid():N}",
            Email: $"comprador_{Guid.NewGuid():N}@marketplace.com",
            Senha: "Senha@123",
            Documento: NovoCpfTeste(),
            TipoPessoa: ETipoPessoa.PessoaFisica,
            Funcao: EUsuarioFuncao.Comprador);
        var clienteComprador = NovoCliente();
        var (_, loginComprador) = await CriarEAutenticarAsync(clienteComprador, compradorRequest);

        clienteComprador.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginComprador.AccessToken);
        var resposta = await clienteComprador.DeleteAsync($"/api/usuario/{usuarioAlvo.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task QuandoObterUsuarioAposDeletar_DeveRetornar404()
    {
        var cliente = NovoCliente();
        var (usuario, login) = await CriarEAutenticarAsync(cliente);

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        await cliente.DeleteAsync($"/api/usuario/{usuario.Id}");
        var resposta = await cliente.GetAsync($"/api/usuario/{usuario.Id}");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    // ── Fluxo completo (happy path de ponta a ponta) ─────────────────────────

    [Fact]
    public async Task FluxoCompleto_CriarLoginRefreshAtualizarResetarSenhaDeletar()
    {
        var cliente = NovoCliente();

        // 1. Criar
        var request = NovoUsuarioAdministrador();
        var respostaCriacao = await cliente.PostAsJsonAsync("/api/usuario", request);
        Assert.Equal(HttpStatusCode.Created, respostaCriacao.StatusCode);
        var usuario = await respostaCriacao.Content.ReadFromJsonAsync<UsuarioDto>();
        Assert.NotNull(usuario);
        Assert.Equal(request.Nome, usuario.Nome);
        Assert.Equal(request.Email.ToLower(), usuario.Email);
        Assert.Equal(EUsuarioFuncao.Administrador, usuario.Funcao);

        // 2. Login
        var respostaLogin = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { request.Email, request.Senha });
        Assert.Equal(HttpStatusCode.OK, respostaLogin.StatusCode);
        var login = await respostaLogin.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);

        // 3. GET com token
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
        var respostaGet = await cliente.GetAsync($"/api/usuario/{usuario.Id}");
        Assert.Equal(HttpStatusCode.OK, respostaGet.StatusCode);
        var usuarioObtido = await respostaGet.Content.ReadFromJsonAsync<UsuarioDto>();
        Assert.Equal(usuario.Id, usuarioObtido!.Id);

        // 4. Refresh token
        var respostaRefresh = await cliente.PostAsJsonAsync("/api/autenticacao/refresh-token",
            new { Token = login.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, respostaRefresh.StatusCode);
        var novoToken = await respostaRefresh.Content.ReadFromJsonAsync<RefreshTokenResponse>();
        Assert.NotNull(novoToken);

        // 5. Atualizar com token renovado
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", novoToken.AccessToken);
        var novoEmail = $"atualizado_{Guid.NewGuid():N}@marketplace.com";
        var respostaAtualizacao = await cliente.PutAsJsonAsync($"/api/usuario/{usuario.Id}",
            new { Nome = "Nome Pos-Atualizacao", Email = novoEmail });
        Assert.Equal(HttpStatusCode.OK, respostaAtualizacao.StatusCode);

        // 6. Resetar senha
        cliente.DefaultRequestHeaders.Authorization = null;
        await cliente.PostAsJsonAsync("/api/autenticacao/esqueci-senha", new { Email = novoEmail });
        var tokenRedefinicao = factory.EmailFake.ExtrairToken();
        const string novaSenha = "NovaSenha@456";
        var respostaReset = await cliente.PostAsJsonAsync("/api/autenticacao/resetar-senha",
            new { Email = novoEmail, Token = tokenRedefinicao, NovaSenha = novaSenha });
        Assert.Equal(HttpStatusCode.NoContent, respostaReset.StatusCode);

        // 7. Login com nova senha
        var respostaLoginNovaSenha = await cliente.PostAsJsonAsync("/api/autenticacao/login",
            new { Email = novoEmail, Senha = novaSenha });
        Assert.Equal(HttpStatusCode.OK, respostaLoginNovaSenha.StatusCode);
        var loginNovaSenha = await respostaLoginNovaSenha.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginNovaSenha);

        // 8. Deletar
        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginNovaSenha.AccessToken);
        var respostaDeletar = await cliente.DeleteAsync($"/api/usuario/{usuario.Id}");
        Assert.Equal(HttpStatusCode.NoContent, respostaDeletar.StatusCode);

        // 9. GET após deleção
        var respostaGetAposDeletar = await cliente.GetAsync($"/api/usuario/{usuario.Id}");
        Assert.Equal(HttpStatusCode.NotFound, respostaGetAposDeletar.StatusCode);
    }
}
