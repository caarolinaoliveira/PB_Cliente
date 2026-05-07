# PB.Cliente — MS Cadastro de Clientes

Microsserviço responsável pelo cadastro de clientes via API REST. Ao registrar um cliente, publica o evento `ClienteCadastrado` no RabbitMQ para ser consumido pelo MS Proposta.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core + SQL Server
- RabbitMQ.Client 6.8.1
- Swagger/OpenAPI
- Clean Architecture + DDD

## Estrutura

```
PB.Cliente/
  PB.Cliente.Domain/          # Entidades, interfaces, exceções
  PB.Cliente.Application/     # Use cases, services, DTOs, eventos
  PB.Cliente.Infrastructure/  # EF Core, repositórios, RabbitMQ
  PB.Cliente.Presentation/    # Controllers, middlewares, Program.cs
  Testes/
    PB.Cliente.Application.Tests/  # Testes unitários com xUnit
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Como rodar localmente

### 1. Subir a infraestrutura

Na raiz da solution (onde está o `docker-compose.yml`), sobe o RabbitMQ e o SQL Server:

```bash
docker-compose up -d rabbitmq sqlserver
```

Confirma que os containers estão rodando:

```bash
docker ps
```

Você deve ver:

```
pb_rabbitmq    → portas 5672 e 15672
pb_sqlserver   → porta 1433
```

> Painel do RabbitMQ disponível em http://localhost:15672 com usuário `guest` e senha `guest`

### 2. Configurar o appsettings

No arquivo `PB.Cliente.Presentation/appsettings.json`, configure as credenciais de acordo com o ambiente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PB_Clientes;User Id=sa;Password=Pb@123456;TrustServerCertificate=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "User": "guest",
    "Password": "guest"
  }
}
```

> Para usar o Azure SQL Server, substitua a connection string mantendo `Database=PB_Clientes`.

### 3. Aplicar migrations

```bash
cd PB.Cliente/PB.Cliente.Infrastructure
dotnet ef database update --startup-project ../PB.Cliente.Presentation
```

### 4. Rodar a API

```bash
cd PB.Cliente/PB.Cliente.Presentation
dotnet run
```

A API estará disponível em `http://localhost:5260/swagger/index.html`.

### 5. Acessar o Swagger

```
http://localhost:5260/swagger/index.html
```

## Infraestrutura Docker

O `docker-compose.yml` sobe dois serviços compartilhados entre todos os microsserviços:

| Container | Imagem | Porta | Uso |
|---|---|---|---|
| pb_rabbitmq | rabbitmq:3-management | 5672 / 15672 | Broker de mensagens + painel web |
| pb_sqlserver | mssql/server:2022 | 1433 | Banco de dados SQL Server |

Os dados são persistidos em volumes Docker (`rabbitmq_data` e `sqlserver_data`) — reiniciar os containers não apaga filas nem bancos.

Comandos úteis:

```bash
# Parar os containers
docker-compose down

# Parar e apagar todos os dados (reset total)
docker-compose down -v

# Ver logs do RabbitMQ
docker logs pb_rabbitmq

# Ver logs do SQL Server
docker logs pb_sqlserver
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| POST | /api/clientes/registrar | Cadastra um novo cliente |
| GET | /api/clientes/{id} | Busca cliente por ID |

## Exemplo de requisição

```json
POST /api/clientes/registrar
{
  "nome": "Carolina Oliveira",
  "email": "carolina@example.com",
  "senha": "Teste123#",
  "confirmacaoSenha": "Teste123#",
  "dataNascimento": "1999-03-15",
  "cpf": "12345678901",
  "telefone": "41999999999"
}
```

## Fluxo do evento

Após o cadastro bem-sucedido, o MS Cliente publica o evento `ClienteCadastrado` na fila `cliente.cadastrado` do RabbitMQ. O MS Proposta consome esse evento para gerar a análise de crédito.

```
POST /api/clientes/registrar
        ↓
Valida CPF e email duplicados
        ↓
Persiste no banco (PB_Clientes)
        ↓
Publica → RabbitMQ: cliente.cadastrado
        ↓
Retorna 201 Created
```

## Testes

```bash
cd PB.Cliente/Testes/PB.Cliente.Application.Tests
dotnet test
```

Cenários cobertos:

- Cadastro com sucesso e validação do evento publicado
- CPF duplicado lança ConflictException
- Email duplicado lança ConflictException
- Busca por ID existente retorna ClienteResponse
- Busca por ID inexistente lança NotFoundException

## Decisões arquiteturais

- **IMessagePublisher**: abstração na camada Application — permite trocar RabbitMQ por Azure Service Bus sem alterar regras de negócio.
- **Validação de CPF e email**: feita na camada Application antes de persistir, retornando `409 Conflict` caso já exista.
- **ExceptionMiddleware**: captura exceções de domínio e retorna respostas HTTP padronizadas.
- **Evento rico**: o `ClienteCadastradoEvent` carrega todos os dados necessários para os serviços downstream, evitando consultas adicionais entre serviços.
- **Retry**: em caso de falha na publicação do evento, o publisher tenta reenviar automaticamente via `BasicNack(requeue: true)`.