# Arquitetura do Sistema - MinhaNotificacao

## Estrutura de Camadas

```
┌─────────────────────────────────────────────────┐
│              Controllers Layer                  │
│  (UsersController, NotificationsController)     │
│                                                 │
│  - Recebe requisições HTTP                      │
│  - Validação de entrada                         │
│  - Retorna respostas HTTP                       │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│               Services Layer                    │
│     (UserService, NotificationService)          │
│                                                 │
│  - Lógica de negócio                           │
│  - Validações de regras                        │
│  - Orquestração de operações                   │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│             Repositories Layer                  │
│   (UserRepository, NotificationRepository)      │
│                                                 │
│  - Acesso a dados                              │
│  - Queries especializadas                      │
│  - CRUD operations                             │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│              Data Layer                         │
│         (ApplicationDbContext)                  │
│                                                 │
│  - DbContext do Entity Framework               │
│  - Configurações de entidades                  │
│  - Mapeamento objeto-relacional                │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│              Database                           │
│         (SQL Server LocalDB)                    │
│                                                 │
│  - Tabelas: Users, Notifications               │
│  - Relacionamentos e constraints               │
└─────────────────────────────────────────────────┘
```

## Fluxo de Dependências

```
Program.cs
   │
   ├─► ApplicationDbContext (DbContext)
   │
   ├─► Repositories
   │   ├─► IUserRepository → UserRepository
   │   └─► INotificationRepository → NotificationRepository
   │
   └─► Services
       ├─► IUserService → UserService
       └─► INotificationService → NotificationService
```

## Estrutura de Pastas

```
MinhaNotificacao/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── UsersController.cs           ← API Controller
│   └── NotificationsController.cs   ← API Controller
│
├── Models/
│   ├── ErrorViewModel.cs
│   ├── User.cs                      ← Entidade
│   └── Notification.cs              ← Entidade
│
├── Data/
│   └── ApplicationDbContext.cs      ← DbContext
│
├── Repositories/
│   ├── Interfaces/
│   │   ├── IRepository.cs           ← Interface base
│   │   ├── IUserRepository.cs       ← Interface específica
│   │   └── INotificationRepository.cs
│   ├── Repository.cs                ← Implementação base
│   ├── UserRepository.cs            ← Implementação específica
│   └── NotificationRepository.cs
│
├── Services/
│   ├── Interfaces/
│   │   ├── IUserService.cs          ← Interface de serviço
│   │   └── INotificationService.cs
│   ├── UserService.cs               ← Implementação de serviço
│   └── NotificationService.cs
│
├── Extensions/
│   └── DatabaseSeeder.cs            ← Dados iniciais
│
├── Views/                           ← Views MVC
├── wwwroot/                         ← Arquivos estáticos
├── Migrations/                      ← Entity Framework Migrations
│
├── Program.cs                       ← Configuração da aplicação
├── appsettings.json                 ← Configurações
└── README.md                        ← Documentação
```

## Relacionamentos entre Entidades

```
┌──────────────────────┐
│       User           │
├──────────────────────┤
│ Id (PK)              │
│ Username             │
│ Email                │
│ PasswordHash         │
│ FullName             │
│ Role                 │
│ CreatedAt            │
│ UpdatedAt            │
│ IsActive             │
│ LastLoginAt          │
└──────────┬───────────┘
           │
           │ 1:N
           │
           │
┌──────────▼───────────┐
│   Notification       │
├──────────────────────┤
│ Id (PK)              │
│ UserId (FK)          │
│ Title                │
│ Message              │
│ Type                 │
│ IsRead               │
│ CreatedAt            │
│ ReadAt               │
│ Link                 │
└──────────────────────┘
```

## Padrão Repository - Hierarquia de Interfaces

```
IRepository<T>
    │
    ├── GetByIdAsync()
    ├── GetAllAsync()
    ├── FindAsync()
    ├── AddAsync()
    ├── UpdateAsync()
    ├── DeleteAsync()
    ├── ExistsAsync()
    └── CountAsync()
    
    ├─► IUserRepository
    │      ├── GetByEmailAsync()
    │      ├── GetByUsernameAsync()
    │      ├── GetActiveUsersAsync()
    │      └── GetUsersByRoleAsync()
    │
    └─► INotificationRepository
           ├── GetByUserIdAsync()
           ├── GetUnreadByUserIdAsync()
           ├── GetByTypeAsync()
           ├── GetUnreadCountByUserIdAsync()
           ├── MarkAsReadAsync()
           └── MarkAllAsReadByUserIdAsync()
```

## Injeção de Dependências

```
Controller
    │
    └─► injeta IUserService
              │
              └─► injeta IUserRepository
                        │
                        └─► injeta ApplicationDbContext
```

## Ciclo de Vida dos Serviços (Scoped)

```
HTTP Request
    │
    ├─► Cria novo Scope
    │       │
    │       ├─► Instancia ApplicationDbContext
    │       ├─► Instancia Repositories
    │       ├─► Instancia Services
    │       └─► Instancia Controllers
    │
    ├─► Processa Request
    │
    └─► Descarta Scope
            │
            └─► Libera recursos (DbContext, etc.)
```

## Fluxo de uma Requisição API

```
1. HTTP POST /api/users
        │
        ▼
2. UsersController.Create()
        │
        ▼
3. IUserService.CreateUserAsync()
        │
        ├─► Validação de Email único
        ├─► Validação de Username único
        │
        ▼
4. IUserRepository.AddAsync()
        │
        ▼
5. ApplicationDbContext.SaveChangesAsync()
        │
        ▼
6. SQL Server - INSERT INTO Users
        │
        ▼
7. Retorna User criado
        │
        ▼
8. HTTP 201 Created com objeto User
```

## Tipos de Dados e Validações

### User
- **Username**: String (max 100) - Único, Required
- **Email**: String (max 200) - Único, Required, EmailAddress
- **PasswordHash**: String (max 500) - Required
- **FullName**: String (max 200)
- **Role**: String (max 50) - "Admin" ou "NormalUser"
- **IsActive**: Boolean - Default: true
- **CreatedAt**: DateTime - Default: UtcNow
- **UpdatedAt**: DateTime - Default: UtcNow
- **LastLoginAt**: DateTime? - Nullable

### Notification
- **UserId**: Integer - Foreign Key, Required
- **Title**: String (max 200) - Required
- **Message**: String (max 1000) - Required
- **Type**: String (max 50) - "Info", "Warning", "Error", "Success"
- **IsRead**: Boolean - Default: false
- **CreatedAt**: DateTime - Default: UtcNow
- **ReadAt**: DateTime? - Nullable
- **Link**: String (max 500) - Nullable

## Índices do Banco de Dados

```sql
-- Tabela Users
- PRIMARY KEY: Id
- UNIQUE INDEX: Email
- UNIQUE INDEX: Username

-- Tabela Notifications
- PRIMARY KEY: Id
- INDEX: UserId
- INDEX: IsRead
- INDEX: CreatedAt
- FOREIGN KEY: UserId → Users(Id) ON DELETE CASCADE
```

## Migrations

```
InitialCreate (gerada)
    │
    ├─► Cria tabela Users
    ├─► Cria tabela Notifications
    ├─► Cria índices
    ├─► Cria foreign keys
    └─► Configura defaults
```

## Configurações de Aplicação (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## Extensibilidade

### Para adicionar nova entidade:

1. Criar Model em `Models/`
2. Adicionar DbSet em `ApplicationDbContext`
3. Configurar entidade em `OnModelCreating()`
4. Criar Interface Repository em `Repositories/Interfaces/`
5. Criar Repository em `Repositories/`
6. Criar Interface Service em `Services/Interfaces/`
7. Criar Service em `Services/`
8. Registrar em `Program.cs`
9. Criar Controller em `Controllers/`
10. Criar Migration: `dotnet ef migrations add NomeDescritivo`
11. Atualizar DB: `dotnet ef database update`
