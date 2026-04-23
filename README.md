# Marketplace.Auth.API

API RESTful de autenticação e gerenciamento de usuários para o ecossistema Marketplace.  
Desenvolvida em **.NET 10**, seguindo os princípios de **Clean Architecture** e **CQRS** com MediatR.

---

## 🛠️ Tecnologias

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (PostgreSQL)
- MediatR (CQRS)
- JWT Bearer Authentication
- BCrypt (hash de senha)
- SMTP (envio de e-mail via Gmail ou Resend)

---

## ⚙️ Configuração

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "Padrao": "Host=localhost;Port=5432;Database=Marketplace.Auth.Db;Username=postgres;Password=sua_senha"
  },
  "Jwt": {
    "Chave": "sua-chave-secreta-longa",
    "Emissor": "Marketplace.Auth.API",
    "Audiencia": "Marketplace.Auth.Clientes",
    "ExpiracaoMinutos": "60",
    "RefreshTokenExpiracaoDias": "7"
  },
  "Database": {
    "RecriarAoIniciar": false
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Porta": "587",
    "UsarSsl": "true",
    "Usuario": "seu-email@gmail.com",
    "Senha": "sua-app-password",
    "Remetente": "seu-email@gmail.com"
  }
}
```

> ⚠️ Em desenvolvimento, `appsettings.Development.json` pode sobrescrever `Database:RecriarAoIniciar`.  
> Mantenha como `false` para preservar os dados entre execuções.  
> Use `true` apenas quando houver mudanças no modelo de dados.

### SMTP com Gmail

Para usar o Gmail como servidor de envio:
1. Ative a **verificação em duas etapas** na conta Google
2. Acesse [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords)
3. Gere uma **App Password** e use-a no campo `Senha`

---

## 🗃️ Entidades

### Usuário

| Campo      | Tipo             | Descrição                                       |
|------------|------------------|-------------------------------------------------|
| `id`       | `Guid`           | Identificador único                             |
| `nome`     | `string`         | Nome completo                                   |
| `email`    | `string`         | E-mail (armazenado em lowercase)                |
| `funcao`   | `EUsuarioFuncao` | Papel do usuário no sistema (ver tabela abaixo) |
| `status`   | `EUsuarioStatus` | Situação da conta (ver tabela abaixo)           |
| `criadoEm` | `DateTime`       | Data de criação (UTC)                           |

#### `funcao` — EUsuarioFuncao

| Valor | Nome            | Descrição                                    |
|-------|-----------------|----------------------------------------------|
| `0`   | `Comprador`     | Usuário padrão, pode realizar compras        |
| `1`   | `Vendedor`      | Pode cadastrar e gerenciar produtos          |
| `2`   | `Administrador` | Acesso total, incluindo exclusão de usuários |

#### `status` — EUsuarioStatus

| Valor | Nome       | Descrição                          |
|-------|------------|------------------------------------|
| `0`   | `Ativo`    | Conta ativa, acesso liberado       |
| `1`   | `Inativo`  | Conta desativada                   |
| `2`   | `Suspenso` | Conta suspensa administrativamente |

---

## 🔐 Autenticação

A API utiliza **JWT Bearer Token**.  
Inclua o header abaixo em todas as requisições protegidas:

```
Authorization: Bearer {accessToken}
```

O `accessToken` expira conforme `Jwt:ExpiracaoMinutos` (padrão: 60 minutos).  
Use o endpoint `refresh-token` para renová-lo sem precisar fazer login novamente.

---

## 📡 Endpoints

**Base URL:** `https://localhost:8100`

---

### 👤 Usuários — `/api/Usuario`

#### `POST /api/Usuario` — Criar usuário

Não requer autenticação.

**Body:**
```json
{
  "nome": "Ronaldo Grillo",
  "email": "usuario@email.com",
  "senha": "Senha@123",
  "funcao": 0
}
```

**Resposta:** `201 Created`
```json
{
  "id": "a7ebcf6b-dc0c-4d43-ae77-eff70a77fc64",
  "nome": "Ronaldo Grillo",
  "email": "usuario@email.com",
  "funcao": 0,
  "status": 0,
  "criadoEm": "2026-04-23T14:54:38Z"
}
```

---

#### `GET /api/Usuario/{id}` — Obter usuário por ID

🔒 Requer autenticação.

**Resposta:** `200 OK` — retorna `UsuarioDto`

---

#### `PUT /api/Usuario/{id}` — Atualizar usuário

🔒 Requer autenticação.

**Body:**
```json
{
  "nome": "Novo Nome",
  "email": "novo@email.com"
}
```

**Resposta:** `200 OK` — retorna `UsuarioDto` atualizado

---

#### `DELETE /api/Usuario/{id}` — Deletar usuário

🔒 Requer autenticação + role `Administrador`.

**Resposta:** `204 No Content`

---

### 🔑 Autenticação — `/api/Autenticacao`

#### `POST /api/Autenticacao/login` — Login

Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@email.com",
  "senha": "Senha@123"
}
```

**Resposta:** `200 OK`
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "GC5KxMwi...",
  "expiraEm": "2026-04-30T15:01:18Z",
  "usuarioId": "a7ebcf6b-dc0c-4d43-ae77-eff70a77fc64",
  "nome": "Ronaldo Grillo",
  "email": "usuario@email.com"
}
```

---

#### `POST /api/Autenticacao/refresh-token` — Renovar tokens

Não requer autenticação.

**Body:**
```json
{
  "token": "seu_refresh_token_aqui"
}
```

> ⚠️ O `refreshToken` é gerado em Base64 e pode conter `+`, `/` e `=`.  
> Sempre envie dentro de um body JSON — nunca via query string ou URL.

**Resposta:** `200 OK`
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "novo_refresh_token...",
  "expiraEm": "2026-04-30T16:00:00Z"
}
```

> O refresh token anterior é **revogado** automaticamente a cada renovação.

---

#### `POST /api/Autenticacao/esqueci-senha` — Solicitar redefinição de senha

Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@email.com"
}
```

**Resposta:** `204 No Content`

> Um e-mail é enviado com um token alfanumérico válido por **2 horas**.

---

#### `POST /api/Autenticacao/resetar-senha` — Redefinir senha

Não requer autenticação.

**Body:**
```json
{
  "email": "usuario@email.com",
  "token": "449733fe7bd24393ac72ed975993b1fe",
  "novaSenha": "NovaSenha@456"
}
```

**Resposta:** `204 No Content`

---

## ❌ Respostas de Erro

| Status | Situação                                        |
|--------|-------------------------------------------------|
| `400`  | Dados inválidos ou regra de negócio violada     |
| `401`  | Credenciais inválidas ou token ausente/expirado |
| `404`  | Usuário não encontrado                          |
| `500`  | Erro interno do servidor                        |

**Exemplo `400` (validação):**
```json
{
  "erros": [
    { "campo": "Email", "mensagem": "E-mail inválido." }
  ]
}
```

**Exemplo `401`:**
```json
{
  "erro": "E-mail ou senha inválidos."
}
```

---

## 🔁 Fluxo Completo de Uso

```
1. POST /api/Usuario                        → Criar conta
2. POST /api/Autenticacao/login             → Obter accessToken + refreshToken
3. GET  /api/Usuario/{id}                   → Usar accessToken no header Authorization
4. POST /api/Autenticacao/refresh-token     → Renovar tokens antes de expirar
5. POST /api/Autenticacao/esqueci-senha     → Solicitar reset por e-mail
6. POST /api/Autenticacao/resetar-senha     → Redefinir com o token recebido
```

---

## 📁 Estrutura do Projeto

```
Marketplace.Auth.API/           → Controllers, Middlewares, Program.cs
Marketplace.Auth.Aplicacao/     → UseCases (CQRS), DTOs, Interfaces
Marketplace.Auth.Dominio/       → Entidades, Enums, Exceções, Interfaces
Marketplace.Auth.Repositorio/   → EF Core, Repositórios, Serviços (JWT, Email, BCrypt)
```
