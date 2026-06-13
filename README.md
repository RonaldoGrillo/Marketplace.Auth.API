# Marketplace.Auth.API

API RESTful de autenticacao e gerenciamento de usuarios para o ecossistema Marketplace.

---

## Sumario

- [Visao Geral](#visao-geral)
- [Configuracao](#configuracao)
- [Ciclo de Vida dos Tokens](#ciclo-de-vida-dos-tokens)
- [Endpoints](#endpoints)
  - [Autenticacao](#autenticacao)
  - [Usuarios](#usuarios)
- [Guia de Integracao para Outros Microsservicos](#guia-de-integracao-para-outros-microsservicos)
- [Fluxo Completo de Autenticacao](#fluxo-completo-de-autenticacao)
- [Respostas de Erro](#respostas-de-erro)

---

## Visao Geral

Esta API e responsavel exclusivamente por:

1. Registrar e gerenciar usuarios.
2. Autenticar usuarios e emitir tokens JWT (access token) e refresh tokens.
3. Renovar access tokens usando um refresh token valido.
4. Encerrar sessoes especificas (logout).
5. Redefinir senhas via e-mail.

Todas as outras APIs do Marketplace **validam o JWT localmente** â€” sem precisar chamar esta API a cada requisicao.

---

## Configuracao

```json
{
  "Jwt": {
	"Chave": "<chave-secreta-forte>",
	"Emissor": "Marketplace.Auth.API",
	"Audiencia": "Marketplace.Auth.Clientes",
	"ExpiracaoMinutos": "60",
	"RefreshTokenExpiracaoDias": "7"
  }
}
```

| Parametro                   | Descricao                                          | Padrao |
|-----------------------------|----------------------------------------------------|--------|
| `Chave`                     | Chave HMAC-SHA256 para assinar o JWT               | â€”      |
| `Emissor`                   | `iss` claim do token                               | â€”      |
| `Audiencia`                 | `aud` claim do token                               | â€”      |
| `ExpiracaoMinutos`          | Validade do access token em minutos                | `60`   |
| `RefreshTokenExpiracaoDias` | Validade do refresh token em dias                  | `7`    |

> **Outras APIs:** configure `AddJwtBearer` com a mesma `Chave`, `Emissor` e `Audiencia` â€” veja o exemplo em [Guia de Integracao](#guia-de-integracao-para-outros-microsservicos).

---

## Ciclo de Vida dos Tokens

```
Login
  >> Novo Access Token  (JWT, valido por ExpiracaoMinutos â€” padrao 60 min)
  >> Novo Refresh Token (opaco, valido por RefreshTokenExpiracaoDias â€” padrao 7 dias)
  >> Sessoes anteriores NAO sao invalidadas (multi-sessao ativo)

Enquanto o Refresh Token for valido:
  POST /api/autenticacao/refresh-token
	>> Novo Access Token  [gerado]
	>> Refresh Token      [o mesmo â€” nao e rotacionado]

POST /api/autenticacao/logout
  >> Refresh Token informado e revogado imediatamente
  >> Outras sessoes do mesmo usuario NAO sao afetadas

Refresh Token expirado (apos 7 dias):
  >> Usuario deve fazer login novamente
```

**Regras importantes:**

- **Multi-sessao:** cada login gera um refresh token independente. Celular, notebook e outros clientes podem estar logados simultaneamente.
- O refresh token **nao e rotacionado** a cada renovacao. O mesmo token continua valido ate expirar ou ser revogado via logout.
- Logout **apenas revoga o token informado** â€” nao afeta outras sessoes.
- Refresh token expirado ou revogado â†’ a API retorna `400` â†’ o cliente deve redirecionar para login.

---

## Endpoints

### Autenticacao

#### `POST /api/autenticacao/login`

Autentica o usuario e retorna os tokens. Nao requer autenticacao.

**Body:**
```json
{
  "email": "usuario@exemplo.com",
  "senha": "Senha@123"
}
```

**Resposta `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "accessTokenExpiresIn": "2026-01-15T11:00:00Z",
  "refreshToken": "GC5KxMwi...",
  "refreshTokenExpiresIn": "2026-01-22T10:00:00Z",
  "usuarioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Joao Silva",
  "email": "usuario@exemplo.com"
}
```

| Campo                    | Descricao                                     |
|--------------------------|-----------------------------------------------|
| `accessToken`            | JWT para usar no header `Authorization`       |
| `accessTokenExpiresIn`   | Quando o access token expira (UTC)            |
| `refreshToken`           | Token opaco para renovar o access token       |
| `refreshTokenExpiresIn`  | Quando o refresh token expira (UTC)           |
| `usuarioId`              | ID do usuario autenticado                     |
| `nome`                   | Nome do usuario                               |
| `email`                  | E-mail do usuario                             |

> O `refreshToken` e gerado em Base64 e pode conter `+`, `/` e `=`.
> Sempre envie dentro de um body JSON â€” nunca via query string ou URL.

---

#### `POST /api/autenticacao/refresh-token`

Renova o access token usando um refresh token valido. **O refresh token nao muda.**
Nao requer autenticacao.

**Body:**
```json
{
  "token": "<refresh-token>"
}
```

**Resposta `200 OK`:**
```json
{
  "accessToken": "eyJhbGci...",
  "accessTokenExpiresIn": "2026-01-15T12:00:00Z"
}
```

| Campo                   | Descricao                               |
|-------------------------|-----------------------------------------|
| `accessToken`           | Novo JWT                                |
| `accessTokenExpiresIn`  | Quando o novo access token expira (UTC) |

**Erros:**

| Status | Situacao                                   |
|--------|--------------------------------------------|
| `400`  | Refresh token invalido ou expirado         |
| `404`  | Usuario associado ao token nao encontrado  |

---

#### `POST /api/autenticacao/logout`

Invalida o refresh token da sessao atual. Nao afeta outras sessoes do mesmo usuario.
Nao requer autenticacao â€” funciona mesmo com access token expirado.

**Body:**
```json
{
  "token": "<refresh-token>"
}
```

**Resposta:** `204 No Content`

> Retorna `204` independentemente de o token existir ou ja estar revogado â€” comportamento intencional para evitar enumeracao de tokens.

---

#### `POST /api/autenticacao/esqueci-senha`

Envia um e-mail com token para redefinicao de senha (valido por 2 horas).
Nao requer autenticacao.

**Body:**
```json
{
  "email": "usuario@exemplo.com"
}
```

**Resposta:** `204 No Content`

---

#### `POST /api/autenticacao/resetar-senha`

Redefine a senha usando o token recebido por e-mail.
Nao requer autenticacao.

**Body:**
```json
{
  "email": "usuario@exemplo.com",
  "token": "449733fe7bd24393ac72ed975993b1fe",
  "novaSenha": "NovaSenha@456"
}
```

**Resposta:** `204 No Content`

---

### Usuarios

#### `POST /api/usuario`

Cria um novo usuario. Nao requer autenticacao.

**Body:**
```json
{
  "nome": "Joao Silva",
  "email": "joao@exemplo.com",
  "senha": "Senha@123",
  "documento": "01234567890",
  "tipoPessoa": 0,
  "funcao": 0,
  "nomeFantasia": null,
  "dataNascimento": "1990-05-20",
  "telefone": "54991234567"
}
```

| Campo           | Tipo       | Obrigatorio | Descricao                                               |
|-----------------|------------|-------------|---------------------------------------------------------|
| `nome`          | string     | Sim         | Nome completo                                           |
| `email`         | string     | Sim         | E-mail unico                                            |
| `senha`         | string     | Sim         | Minimo 8 caracteres, letras maiusculas, numeros e simbolos |
| `documento`     | string     | Sim         | CPF (PF) ou CNPJ (PJ), somente digitos                 |
| `tipoPessoa`    | int        | Sim         | `0` = PessoaFisica, `1` = PessoaJuridica               |
| `funcao`        | int        | Nao         | `0` = Comprador (padrao), `1` = Vendedor, `2` = Administrador |
| `nomeFantasia`  | string     | Nao         | Apenas para PessoaJuridica                              |
| `dataNascimento`| DateOnly   | Nao         | Formato `YYYY-MM-DD`                                    |
| `telefone`      | string     | Nao         | Somente digitos                                         |

**Resposta `201 Created`:**
```json
{
  "id": "a7ebcf6b-dc0c-4d43-ae77-eff70a77fc64",
  "nome": "Joao Silva",
  "nomeFantasia": null,
  "email": "joao@exemplo.com",
  "documento": "01234567890",
  "dataNascimento": "1990-05-20",
  "telefone": "54991234567",
  "tipoPessoa": 0,
  "funcao": 0,
  "status": 0,
  "criadoEm": "2026-04-23T14:54:38Z"
}
```

---

#### `GET /api/usuario/{id}`

Retorna os dados de um usuario pelo ID.
Requer autenticacao.

**Resposta `200 OK`:** `UsuarioDto` (mesmo schema do `POST /api/usuario`)

---

#### `PUT /api/usuario/{id}`

Atualiza os dados de um usuario.
Requer autenticacao.

**Body:**
```json
{
  "nome": "Joao Atualizado",
  "email": "novo@exemplo.com",
  "nomeFantasia": null,
  "dataNascimento": "1990-05-20",
  "telefone": "54999998888"
}
```

> `nomeFantasia`, `dataNascimento` e `telefone` sao opcionais.
> `documento` e `tipoPessoa` nao podem ser alterados apos o cadastro.

**Resposta `200 OK`:** `UsuarioDto` atualizado

---

#### `DELETE /api/usuario/{id}`

Remove um usuario.
Requer autenticacao com role `Administrador`.

**Resposta:** `204 No Content`

---

## Guia de Integracao para Outros Microsservicos

### 1. Validar o JWT localmente

Configure `AddJwtBearer` em cada servico com os mesmos parametros desta API:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration["Jwt:Chave"]!)),
			ValidateIssuer = true,
			ValidIssuer = "Marketplace.Auth.API",
			ValidateAudience = true,
			ValidAudience = "Marketplace.Auth.Clientes",
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
	});
```

> Com `ClockSkew = TimeSpan.Zero`, o token expira exatamente em `accessTokenExpiresIn`, sem margem extra.

### 2. Claims disponiveis no token

| Claim   | Conteudo             |
|---------|----------------------|
| `sub`   | `usuarioId` (Guid)   |
| `email` | E-mail do usuario    |
| `name`  | Nome do usuario      |
| `role`  | Funcao do usuario    |
| `jti`   | ID unico do token    |

### 3. Tratamento de 401 nos clientes

```
1. Chamada para qualquer API  â†’  401 Unauthorized
2.   POST /api/autenticacao/refresh-token
		200 OK  â†’  salvar novo accessToken e repetir a chamada original
		400     â†’  refresh token expirado/revogado â†’ redirecionar para login
```

Nunca tente renovar o token de forma recursiva â€” se o refresh tambem falhar, force o logout.

---

## Fluxo Completo de Autenticacao

```
Cliente (dispositivo A)          Auth API                Outra API
  |                                  |                       |
  |-- POST /login ------------------->|                       |
  |<-- { accessToken,                 |                       |
  |      accessTokenExpiresIn,        |                       |
  |      refreshToken,                |                       |
  |      refreshTokenExpiresIn }      |                       |
  |                                   |                       |
  |  (dispositivo B tambem pode       |                       |
  |   fazer login independentemente)  |                       |
  |                                   |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 200 OK ---------------------------------------------- |
  |                                   |                       |
  |   (60 min depois)                 |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 401 Unauthorized ------------------------------------- |
  |                                   |                       |
  |-- POST /refresh-token ----------->|                       |
  |<-- { accessToken,                 |                       |
  |      accessTokenExpiresIn }       |                       |
  |                                   |                       |
  |-- GET /recurso (Bearer) -------------------------------->  |
  |<-- 200 OK ---------------------------------------------- |
  |                                   |                       |
  |   (usuario clica em "sair")       |                       |
  |-- POST /logout (refreshToken) --->|                       |
  |<-- 204 No Content ----------------|                       |
  |                                   |                       |
  |   (7 dias depois â€” RT expirado,   |                       |
  |    ou usuario fez logout)         |                       |
  |-- POST /refresh-token ----------->|                       |
  |<-- 400 Bad Request ---------------|                       |
  |                                   |                       |
  |-- [redirecionar para login]       |                       |
```

---

## Respostas de Erro

| Status | Situacao                                         |
|--------|--------------------------------------------------|
| `400`  | Dados invalidos ou regra de negocio violada      |
| `401`  | Credenciais invalidas ou token ausente/expirado  |
| `404`  | Usuario nao encontrado                           |
| `500`  | Erro interno do servidor                         |

**Exemplo `400` (validacao):**
```json
{
  "erros": [
	{ "campo": "Email", "mensagem": "E-mail invalido." }
  ]
}
```

**Exemplo `401`:**
```json
{
  "erro": "E-mail ou senha invalidos."
}
```