# Marketplace.Auth.API

API RESTful de autenticação e gerenciamento de usuários para o ecossistema Marketplace.

**Base URL:** `http://usuario.neurosky.com.br`

---

## 🗃️ Entidades

### Usuário

| Campo           | Tipo             | Obrigatório | Descrição                                                                 |
|-----------------|------------------|-------------|---------------------------------------------------------------------------|
| `id`            | `Guid`           | Sim         | Identificador único                                                       |
| `nome`          | `string`         | Sim         | Nome completo (Pessoa Física) ou Razão Social (Pessoa Jurídica), máx. 200 |
| `nomeFantasia`  | `string?`        | Não         | Nome fantasia — recomendado para Pessoa Jurídica, máx. 200                |
| `email`         | `string`         | Sim         | E-mail único (armazenado em lowercase)                                    |
| `documento`     | `string`         | Sim         | CPF (PF, 11 dígitos) ou CNPJ (PJ, 14 dígitos) — único, sem formatação    |
| `dataNascimento`| `DateOnly?`      | Não         | Data de nascimento (PF) ou data de fundação (PJ)                          |
| `telefone`      | `string?`        | Não         | Telefone com DDD — apenas dígitos, mín. 10 dígitos                        |
| `tipoPessoa`    | `ETipoPessoa`    | Sim         | Tipo de pessoa (ver tabela abaixo)                                        |
| `funcao`        | `EUsuarioFuncao` | Sim         | Papel no sistema (ver tabela abaixo)                                      |
| `status`        | `EUsuarioStatus` | Sim         | Situação da conta (ver tabela abaixo)                                     |
| `criadoEm`      | `DateTime`       | Sim         | Data de criação (UTC)                                                     |

#### `tipoPessoa` — ETipoPessoa

| Valor | Nome             | Descrição                              |
|-------|------------------|----------------------------------------|
| `0`   | `PessoaFisica`   | Cadastro com CPF e nome completo       |
| `1`   | `PessoaJuridica` | Cadastro com CNPJ e razão social       |

#### `funcao` — EUsuarioFuncao

| Valor | Nome            | Descrição                                    |
|-------|-----------------|----------------------------------------------|
| `0`   | `Comprador`     | Usuário padrão, pode realizar compras        |
| `1`   | `Vendedor`      | Pode cadastrar e gerenciar produtos          |
| `2`   | `Administrador` | Acesso total, incluindo exclusão de usuários |

> `tipoPessoa` e `funcao` são independentes: um Vendedor pode ser PF ou PJ; um Comprador também.

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

**Base URL:** `http://usuario.neurosky.com.br`

---

### 👤 Usuários — `/api/Usuario`

#### `POST /api/Usuario` — Criar usuário

Não requer autenticação.

**Body — Pessoa Física:**
```json
{
  "nome": "Ronaldo Grillo",
  "email": "usuario@email.com",
  "senha": "Senha@123",
  "documento": "01234567890",
  "tipoPessoa": 0,
  "funcao": 0,
  "dataNascimento": "2004-01-08",
  "telefone": "54991283598"
}
```

**Body — Pessoa Jurídica:**
```json
{
  "nome": "Acme Comércio Ltda",
  "nomeFantasia": "Acme Shop",
  "email": "contato@acme.com",
  "senha": "Senha@123",
  "documento": "01234567000195",
  "tipoPessoa": 1,
  "funcao": 1,
  "dataNascimento": "2010-03-20",
  "telefone": "5433330000"
}
```

> - `documento`: apenas dígitos — CPF (11) para `tipoPessoa: 0`, CNPJ (14) para `tipoPessoa: 1`.  
> - `nomeFantasia`, `dataNascimento` e `telefone` são opcionais.  
> - `funcao` padrão é `0` (Comprador) quando omitido.

**Resposta:** `201 Created`
```json
{
  "id": "a7ebcf6b-dc0c-4d43-ae77-eff70a77fc64",
  "nome": "Ronaldo Grillo",
  "nomeFantasia": null,
  "email": "usuario@email.com",
  "documento": "01234567890",
  "dataNascimento": "2004-01-08",
  "telefone": "54991283598",
  "tipoPessoa": 0,
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
  "email": "novo@email.com",
  "nomeFantasia": null,
  "dataNascimento": "1995-06-15",
  "telefone": "54999998888"
}
```

> `nomeFantasia`, `dataNascimento` e `telefone` são opcionais.  
> `documento` e `tipoPessoa` não podem ser alterados após o cadastro.

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
