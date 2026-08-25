# Prometheus V2 — Capstone e Implementação Referencial

## Sobre o projeto

Este repositório contém uma implementação referencial fullstack, pequena e executável, do recorte de **Identidade/Usuários** do Prometheus V2. Seu propósito é apoiar a demonstração acadêmica de autenticação, emissão e validação de JWT, autorização por perfil, proteção de endpoints e rotas, persistência e integração entre frontend e backend.

Esta implementação não corresponde ao LMS completo, não comprova implantação do Prometheus V2 e não deve ser tratada como infraestrutura pronta para produção.

A proposta acadêmica busca modernizar incrementalmente o Prometheus V1, melhorar modularidade, escalabilidade, manutenibilidade e facilidade de evolução, sem pressupor a substituição integral e imediata do legado.

## Objetivo da implementação

O fluxo demonstrado possui somente três perfis:

- **Admin**: autentica-se, acessa a área administrativa, lista usuários e cadastra Professor ou Aluno;
- **Professor**: autentica-se e acessa apenas sua área demonstrativa;
- **Aluno**: autentica-se e acessa apenas sua área demonstrativa.

Não existe cadastro público. Um Admin demonstrativo é criado inicialmente e cadastra os demais usuários. Cursos, matrículas, notas, conteúdos, relatórios, GRA, vídeos e videoconferências estão fora do escopo.

## Relação com a arquitetura do Prometheus V2

O Prometheus V2 permanece documentado como um **monólito modular inicialmente implantado como uma unidade**. Esta implementação cobre apenas o módulo lógico **Users**, com a responsabilidade interna de Identidade — autenticação e autorização — e uma interface web para demonstrá-lo.

O diretório `src/users-api` organiza o contrato HTTP e o código desse recorte. O nome não implica microsserviço, processo independente, banco independente nem unidade separada de implantação. Da mesma forma, os demais diretórios em `src/` representam limites planejados, e não módulos já implementados.

SQLite, JWT e o armazenamento local do token são escolhas deliberadas desta demonstração. Elas não encerram os pontos arquiteturais ainda abertos para o Prometheus V2 completo, como SGBD, Identity Provider, mecanismo de sessão ou plataforma de implantação.

## Documentação arquitetural

A documentação canônica do Capstone, incluindo requisitos, diagramas C4, trade-offs, decisões arquiteturais, cronograma e riscos, permanece na [Wiki do Prometheus V2](https://gitlab.com/jala-university1/cohort-3/PT.CSAR-364.GA.T2.26.M1/SB/erick.lima/capstone-prometheus/-/wikis/home).

Este repositório mantém código-fonte e artefatos versionáveis da implementação. Alterações arquiteturais relevantes devem continuar rastreáveis na Wiki e, quando aplicável, nas fontes editáveis mantidas em `docs/`.

## Estrutura do repositório

```text
.
├── README.md
├── Prometheus.sln
├── .gitignore
├── .config/
│   └── dotnet-tools.json  # dotnet-ef 8.0.14
├── docs/
│   ├── diagrams/
│   │   └── .gitkeep
│   ├── adr/
│   │   └── .gitkeep
│   └── mockups/
│       └── .gitkeep
├── src/
│   ├── users-api/          # implementação ASP.NET Core
│   ├── courses-api/
│   │   └── .gitkeep
│   ├── content-api/
│   │   └── .gitkeep
│   ├── grades-api/
│   │   └── .gitkeep
│   ├── videos-api/
│   │   └── .gitkeep
│   └── portal-web/         # implementação React
└── tests/
    └── users-api-tests/    # testes de integração do backend
```

Os arquivos `.gitkeep` preservam no Git diretórios planejados que ainda não possuem conteúdo. Eles não significam implementação. Em particular, `courses-api`, `content-api`, `grades-api` e `videos-api` estão reservados estruturalmente e **não estão implementados**.

`docs/` também é apenas uma estrutura inicial. Fontes editáveis de diagramas, ADRs e mockups poderão ser incorporadas posteriormente, sem duplicar toda a Wiki. O fluxo previsto para fontes versionáveis é: repositório principal, revisão e publicação ou apresentação na Wiki.

## Tecnologias

### Backend

- C# e .NET 8;
- ASP.NET Core Web API;
- Entity Framework Core;
- SQLite;
- SQLitePCLRaw `2.1.13`, para utilizar o binário SQLite incorporado com correções de segurança;
- JWT Bearer Authentication;
- BCrypt para hash e verificação de senhas;
- Swagger/OpenAPI.

### Frontend

- React;
- TypeScript;
- Vite;
- React Router;
- Axios;
- CSS próprio, sem biblioteca visual pesada.

## Pré-requisitos

- .NET SDK 8;
- Node.js `20.19+` ou `22.12+` e npm, conforme os requisitos do Vite 7;
- `dotnet-ef`, restaurado pelo manifesto local quando forem executados comandos de migrations manualmente.

No Linux, o binário SQLite incorporado e corrigido requer `glibc 2.34+`. Distribuições atuais atendem ao requisito; um SDK .NET empacotado em um Snap antigo baseado em Core20 pode não atendê-lo mesmo quando o host é recente. Nesse caso, use a distribuição oficial atual do SDK .NET 8 (pacote ou arquivo compactado) ou um ambiente compatível.

O repositório fixa `dotnet-ef` em `8.0.14`. Restaure a ferramenta local a partir da raiz:

```bash
dotnet tool restore
```

Os comandos abaixo devem ser executados a partir da raiz deste repositório, salvo quando indicado outro diretório.

## Instalação

Restaure o backend e instale as dependências do frontend:

```bash
dotnet tool restore
dotnet restore
cd src/portal-web
npm install
```

## Banco

A demonstração usa SQLite e cria localmente o arquivo `prometheus-auth.db`. O arquivo de banco e seus auxiliares estão ignorados pelo Git. O modelo persiste `Id`, `Name`, `Email`, `PasswordHash`, `Role` e `CreatedAt`; o e-mail possui restrição de unicidade.

As senhas nunca são persistidas em texto puro: a aplicação armazena somente o hash BCrypt. `PasswordHash` não integra os DTOs de resposta.

## Migrations

As migrations do Entity Framework Core são versionadas. Ao iniciar a API, a aplicação aplica as migrations pendentes. O seed idempotente do Admin demonstrativo é executado somente nos ambientes `Development` e `Testing`; ele não é provisionado automaticamente em outros ambientes.

Também é possível atualizar o banco manualmente:

```bash
cd src/users-api
dotnet restore
dotnet ef database update
```

Não exclua nem ignore o diretório `Migrations/`.

## Executar backend

```bash
cd src/users-api
dotnet restore
dotnet ef database update
dotnet run
```

A API é disponibilizada por padrão em `http://localhost:5000`. O Swagger fica em `http://localhost:5000/swagger` no ambiente de desenvolvimento.

## Executar frontend

Em outro terminal:

```bash
cd src/portal-web
npm install
npm run dev
```

O Portal Web é disponibilizado por padrão em `http://localhost:5173`. A URL da API assume `http://localhost:5000` e pode ser sobrescrita por `VITE_API_URL` em configuração local.

## Executar testes

Na raiz do repositório:

```bash
dotnet build
dotnet test
```

Os testes de integração cobrem autenticação, validação do JWT, criação administrativa, conflito de e-mail, restrição à criação de Admin e autorização entre os três perfis.

Para validar o frontend:

```bash
cd src/portal-web
npm run build
npm run lint
```

## Conta Admin

| Campo | Valor |
|---|---|
| E-mail | `admin@prometheus.local` |
| Senha | `Admin@123` |
| Perfil | `Admin` |

Essa conta existe exclusivamente para desenvolvimento e apresentação acadêmica. A credencial é pública e previsível, portanto não é apropriada para produção. Ambientes reais precisam provisionar administradores e segredos por mecanismos seguros.

## Fluxo de demonstração

1. Inicie o backend.
2. Inicie o frontend.
3. Acesse `/login` e autentique-se como Admin.
4. Cadastre um Professor.
5. Cadastre um Aluno.
6. Verifique os três usuários na listagem administrativa.
7. Encerre a sessão.
8. Autentique-se como Professor e verifique o dashboard e o endpoint exclusivo desse perfil.
9. Tente acessar uma rota ou endpoint incompatível e observe a recusa de autorização.
10. Encerre a sessão.
11. Autentique-se como Aluno e verifique seu dashboard e endpoint exclusivo.
12. Tente acessar as áreas de Professor e Admin.
13. Acesse o Swagger, execute o login, copie o token e use **Authorize**.
14. Demonstre uma resposta `401 Unauthorized` sem token e uma resposta `403 Forbidden` com perfil incompatível.

## Rotas do Portal Web

| Rota | Acesso | Descrição |
|---|---|---|
| `/login` | Público | Autenticação dos três perfis; não oferece cadastro |
| `/admin` | Admin | Dashboard, validação do perfil, cadastro e listagem de usuários |
| `/professor` | Professor | Área demonstrativa exclusiva do Professor |
| `/aluno` | Aluno | Área demonstrativa exclusiva do Aluno |
| `/unauthorized` | Público | Destino informativo de recusas; oferece retorno à área compatível ou ao login |

## Endpoints

| Método | Endpoint | Autorização | Descrição |
|---|---|---|---|
| `POST` | `/api/auth/login` | Público | Valida credenciais e retorna JWT, expiração e usuário |
| `GET` | `/api/auth/me` | Autenticado | Valida o JWT e retorna o usuário atual |
| `GET` | `/api/users` | Admin | Lista usuários sem expor hashes de senha |
| `POST` | `/api/users` | Admin | Cadastra somente Professor ou Aluno |
| `GET` | `/api/demo/authenticated` | Autenticado | Demonstra um endpoint protegido para qualquer perfil válido |
| `GET` | `/api/demo/admin` | Admin | Demonstra autorização exclusiva de Admin |
| `GET` | `/api/demo/professor` | Professor | Demonstra autorização exclusiva de Professor |
| `GET` | `/api/demo/aluno` | Aluno | Demonstra autorização exclusiva de Aluno |

Respostas esperadas incluem `200 OK`, `201 Created`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden` e `409 Conflict`. Por exemplo, credenciais inválidas resultam em `401`, token ausente em `401`, perfil incompatível em `403` e e-mail duplicado em `409`.

## Autenticação e sessão

No login, a API procura o usuário por e-mail e usa `BCrypt.Verify` para comparar a senha recebida com o hash persistido. Credenciais inválidas recebem uma mensagem genérica, sem revelar se o e-mail ou a senha causou a falha.

O JWT assinado contém identificador, e-mail, nome e role. A API valida assinatura, expiração, emissor e audiência. Não há refresh token nesta implementação.

O frontend mantém `user` e `token` em um contexto de autenticação. Ao iniciar com token local, consulta `/api/auth/me`: uma resposta válida restaura a sessão; uma resposta inválida remove o token e redireciona para o login. O logout remove os dados locais.

## Autorização

O frontend protege rotas e redireciona perfis incompatíveis para `/unauthorized`, mas essa proteção existe apenas para navegação e experiência do usuário. A segurança efetiva permanece no backend, com `[Authorize]` e autorização por role em cada endpoint aplicável.

O endpoint administrativo de cadastro aceita somente `Professor` e `Aluno`. Mesmo um Admin autenticado não pode criar outro Admin por esse fluxo.

## Segurança

- senhas são transformadas com BCrypt e nunca armazenadas em texto puro;
- hashes de senha não são expostos pela API, logs ou frontend;
- JWT possui assinatura e expiração e valida `Issuer` e `Audience`;
- a role presente no token é validada pelo backend;
- cadastro e listagem de usuários exigem Admin;
- a API diferencia ausência de autenticação (`401`) de falta de permissão (`403`);
- e-mail duplicado é tratado como conflito (`409`);
- CORS é limitado ao frontend local `http://localhost:5173`;
- Swagger oferece o esquema Bearer para testar os endpoints protegidos.

Por simplicidade didática, o token fica em `localStorage`. Essa escolha facilita a visualização do fluxo, mas amplia o impacto de uma vulnerabilidade XSS. Uma aplicação real deve avaliar cookies `HttpOnly`, `Secure` e `SameSite`, proteção contra CSRF, política de conteúdo, estratégia de sessão e arquitetura de identidade.

A chave de assinatura incluída na configuração é somente de desenvolvimento acadêmico. Segredos reais devem permanecer fora do repositório, por exemplo em variáveis de ambiente, um cofre de segredos ou configuração segura da plataforma. A credencial demonstrativa também deve ser substituída fora do ambiente local.

## Limitações deliberadas

- sem cadastro público;
- sem criação de outros Admins pelo endpoint comum;
- sem refresh token;
- sem recuperação de senha;
- sem MFA;
- sem Identity Provider externo;
- sem gestão de sessão adequada à produção;
- sem cursos, matrículas, notas, conteúdos, relatórios, GRA, vídeos ou videoconferências;
- sem infraestrutura, observabilidade, backup ou recuperação de desastre de produção;
- sem seleção tecnológica válida para o Prometheus V2 completo;
- sem comprovação de implantação ou operação do V2.

## Estado desta entrega

Esta é uma implementação referencial do módulo de Identidade/Usuários. Somente `users-api` e `portal-web` possuem implementação funcional nesta atividade; os demais módulos permanecem reservados.

> Esta implementação representa um recorte referencial do módulo lógico de Identidade/Usuários do monólito modular Prometheus V2. A organização em `users-api` não implica decisão de microsserviço ou implantação independente.
