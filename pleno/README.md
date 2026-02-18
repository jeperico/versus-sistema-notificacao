# MinhaNotificacao - Sistema de Notificações

Sistema ASP.NET Core MVC com Entity Framework Core implementando o padrão Repository para gerenciamento de usuários e notificações.

## 📋 Estrutura do Banco de Dados

### Tabelas

#### Users
- `Id` (int, PK)
- `Username` (string)
- `Email` (string)
- `PasswordHash` (string)
- `FullName` (string)
- `Role` (string: "Admin" ou "NormalUser")
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)
- `IsActive` (bool)
- `LastLoginAt` (datetime, nullable)

#### Notifications
- `Id` (int, PK)
- `UserId` (int, FK → Users)
- `Title` (string)
- `Message` (string)
- `Type` (string: "Info", "Warning", "Error", "Success")
- `IsRead` (bool)
- `CreatedAt` (datetime)
- `ReadAt` (datetime, nullable)
- `Link` (string, nullable)

## 🏗️ Arquitetura

### Camadas

1. **Models** - Entidades do banco de dados
2. **Data** - DbContext do Entity Framework
3. **Repositories** - Camada de acesso a dados (padrão Repository)
4. **Services** - Lógica de negócio
5. **Controllers** - Endpoints da API

### Padrões Implementados

- **Repository Pattern**: Abstração da camada de acesso a dados
- **Dependency Injection**: Injeção de dependências nativa do ASP.NET Core
- **Service Layer**: Camada de serviços para lógica de negócio

## 🚀 Como Usar

### 1. Configurar a Connection String

Edite o arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MinhaNotificacaoDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 2. Criar as Migrations

```powershell
dotnet ef migrations add InitialCreate
```

### 3. Atualizar o Banco de Dados

```powershell
dotnet ef database update
```

### 4. Executar o Projeto

```powershell
dotnet run
```

## 📡 API Endpoints

### Users

- `GET /api/users` - Listar todos os usuários
- `GET /api/users/{id}` - Obter usuário por ID
- `GET /api/users/email/{email}` - Obter usuário por email
- `GET /api/users/active` - Listar usuários ativos
- `GET /api/users/role/{role}` - Listar usuários por role
- `GET /api/users/count` - Contar total de usuários
- `POST /api/users` - Criar novo usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Deletar usuário

### Notifications

- `GET /api/notifications` - Listar todas as notificações
- `GET /api/notifications/{id}` - Obter notificação por ID
- `GET /api/notifications/user/{userId}` - Listar notificações do usuário
- `GET /api/notifications/user/{userId}/unread` - Listar notificações não lidas
- `GET /api/notifications/user/{userId}/unread-count` - Contar notificações não lidas
- `GET /api/notifications/type/{type}` - Listar notificações por tipo
- `GET /api/notifications/count` - Contar total de notificações
- `POST /api/notifications` - Criar nova notificação
- `PUT /api/notifications/{id}` - Atualizar notificação
- `DELETE /api/notifications/{id}` - Deletar notificação
- `PATCH /api/notifications/{id}/mark-as-read` - Marcar como lida
- `PATCH /api/notifications/user/{userId}/mark-all-as-read` - Marcar todas como lidas

## 💡 Exemplos de Uso

### Criar Usuário

```json
POST /api/users
{
  "username": "joao.silva",
  "email": "joao@example.com",
  "passwordHash": "hash_da_senha",
  "fullName": "João Silva",
  "role": "NormalUser"
}
```

### Criar Notificação

```json
POST /api/notifications
{
  "userId": 1,
  "title": "Nova Mensagem",
  "message": "Você tem uma nova mensagem",
  "type": "Info",
  "link": "/messages/123"
}
```

## 🛠️ Tecnologias

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQL Server (LocalDB)
- C# 12

## 📦 Pacotes NuGet

- Microsoft.EntityFrameworkCore.SqlServer (9.0.0)
- Microsoft.EntityFrameworkCore.Tools (9.0.0)
- Microsoft.EntityFrameworkCore.Design (9.0.0)

## 🔧 Comandos Úteis do EF Core

```powershell
# Criar uma nova migration
dotnet ef migrations add NomeDaMigration

# Atualizar o banco de dados
dotnet ef database update

# Remover a última migration
dotnet ef migrations remove

# Listar migrations
dotnet ef migrations list

# Criar script SQL
dotnet ef migrations script

# Remover o banco de dados
dotnet ef database drop
```

## 📝 Observações

- O projeto usa UTC para todas as datas
- Os emails e usernames são únicos no banco de dados
- As notificações são deletadas em cascata quando um usuário é removido
- Os tipos de notificação são: Info, Warning, Error, Success
- Os roles de usuário são: Admin, NormalUser
