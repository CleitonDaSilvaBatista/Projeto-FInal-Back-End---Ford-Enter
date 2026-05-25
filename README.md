# Sistema Bancario - Sprint 3

API REST em ASP.NET Core para gerenciamento de contas bancarias, transacoes, autenticação JWT e interface web responsiva.

## Como executar

1. Configure o MySQL e ajuste `appsettings.json`.
2. Instale dependencias:
   ```bash
   dotnet restore
   ```
3. Crie a migration e aplique no banco:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. Execute:
   ```bash
   dotnet run
   ```
5. Acesse:
   - Swagger: `/swagger`
   - Interface: `/index.html`

## Arquitetura

- Controllers: recebem requisicoes HTTP.
- DTOs: padronizam entrada e saida de dados.
- Models: representam entidades do banco.
- Repositories: acesso ao banco com Entity Framework Core.
- Services: regras de negocio de saque, deposito, taxa e segurança de acesso.

## Regras de negocio

- Conta corrente cobra taxa fixa de R$ 2,50 no saque.
- Conta empresarial cobra 1% do valor no saque.
- Conta poupanca nao cobra taxa.
- Saque so ocorre se houver saldo suficiente.
- O cliente so acessa as proprias contas via JWT.

## Endpoints principais

- POST `/api/auth/registrar`
- POST `/api/auth/login`
- GET `/api/contas`
- POST `/api/contas`
- GET `/api/contas/{id}`
- POST `/api/contas/{id}/depositar`
- POST `/api/contas/{id}/sacar`
- GET `/api/contas/{id}/extrato`
- DELETE `/api/contas/{id}`
