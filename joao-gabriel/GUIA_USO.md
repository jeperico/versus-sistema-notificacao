# Guia de Uso - MinhaNotificacao

## 🎯 Início Rápido

### 1. Primeira Execução

Ao executar o projeto pela primeira vez em modo Development, o sistema automaticamente:
- Criará o banco de dados se não existir
- Aplicará todas as migrations pendentes
- Populará o banco com dados de exemplo

```powershell
dotnet run
```

### 2. Dados de Exemplo Criados

#### Usuários:
1. **admin** (admin@example.com) - Role: Admin
2. **joao.silva** (joao.silva@example.com) - Role: NormalUser
3. **maria.santos** (maria.santos@example.com) - Role: NormalUser

#### Notificações:
- 3 notificações para João Silva (2 não lidas)
- 2 notificações para Maria Santos (1 não lida)

## 📚 Exemplos de Código

### Injetando Services em um Controller

```csharp
public class MeuController : Controller
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public MeuController(
        IUserService userService,
        INotificationService notificationService)
    {
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetActiveUsersAsync();
        return View(users);
    }
}
```

### Criar e Enviar Notificação

```csharp
public async Task<IActionResult> EnviarNotificacao(int userId)
{
    var notification = new Notification
    {
        UserId = userId,
        Title = "Nova Mensagem",
        Message = "Você recebeu uma nova mensagem!",
        Type = "Info",
        Link = "/messages"
    };

    await _notificationService.CreateNotificationAsync(notification);
    return Ok();
}
```

### Marcar Notificações como Lidas

```csharp
// Marcar uma notificação específica
await _notificationService.MarkAsReadAsync(notificationId);

// Marcar todas as notificações de um usuário
await _notificationService.MarkAllAsReadByUserIdAsync(userId);
```

### Buscar Usuário com Notificações

```csharp
var user = await _userService.GetUserByIdAsync(userId);
var notifications = user.Notifications; // Já carregado pelo Include
```

## 🔍 Trabalhando com Repositories Diretamente

Se precisar de mais controle, você pode injetar os repositories diretamente:

```csharp
public class CustomService
{
    private readonly IUserRepository _userRepository;

    public CustomService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetRecentUsers()
    {
        return await _userRepository.FindAsync(u => 
            u.CreatedAt >= DateTime.UtcNow.AddDays(-7));
    }
}
```

## 🎨 Personalizações Comuns

### Alterar Connection String

**Para SQL Server local:**
```json
"Server=localhost;Database=MinhaNotificacaoDB;Trusted_Connection=True;"
```

**Para SQL Server autenticado:**
```json
"Server=localhost;Database=MinhaNotificacaoDB;User Id=sa;Password=SuaSenha123;"
```

**Para Azure SQL:**
```json
"Server=tcp:seuservidor.database.windows.net,1433;Database=MinhaNotificacaoDB;User ID=usuario;Password=senha;"
```

### Adicionar Novos Campos

1. Adicione a propriedade ao Model
2. Crie uma nova Migration:
```powershell
dotnet ef migrations add AdicionarNovoCampo
```
3. Atualize o banco:
```powershell
dotnet ef database update
```

### Criar Novo Service

1. Crie a interface em `Services/Interfaces/`:
```csharp
public interface IMeuService
{
    Task<string> FazerAlgo();
}
```

2. Implemente em `Services/`:
```csharp
public class MeuService : IMeuService
{
    public async Task<string> FazerAlgo()
    {
        return await Task.FromResult("Feito!");
    }
}
```

3. Registre no `Program.cs`:
```csharp
builder.Services.AddScoped<IMeuService, MeuService>();
```

## 🔒 Segurança

### Hash de Senhas

O projeto usa `PasswordHash` como exemplo. Para produção, use:

```csharp
using Microsoft.AspNetCore.Identity;

public class UserService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<User> CreateUserWithHashedPassword(User user, string password)
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        return await _userRepository.AddAsync(user);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
```

Registre no `Program.cs`:
```csharp
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
```

## 📊 Queries Úteis

### Notificações dos últimos 7 dias
```csharp
var notifications = await _notificationRepository.FindAsync(n => 
    n.CreatedAt >= DateTime.UtcNow.AddDays(-7));
```

### Usuários que nunca fizeram login
```csharp
var users = await _userRepository.FindAsync(u => 
    u.LastLoginAt == null);
```

### Contagem de notificações por tipo
```csharp
var infoCount = await _notificationRepository.CountAsync(n => n.Type == "Info");
var errorCount = await _notificationRepository.CountAsync(n => n.Type == "Error");
```

## 🧪 Testando a API

### Usando PowerShell

```powershell
# Listar usuários
Invoke-RestMethod -Uri "https://localhost:5001/api/users" -Method Get

# Criar usuário
$body = @{
    username = "teste"
    email = "teste@example.com"
    passwordHash = "hash123"
    fullName = "Usuário Teste"
    role = "NormalUser"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/users" -Method Post -Body $body -ContentType "application/json"

# Marcar notificação como lida
Invoke-RestMethod -Uri "https://localhost:5001/api/notifications/1/mark-as-read" -Method Patch
```

### Usando Curl

```bash
# Listar notificações
curl https://localhost:5001/api/notifications

# Criar notificação
curl -X POST https://localhost:5001/api/notifications \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "title": "Teste",
    "message": "Mensagem de teste",
    "type": "Info"
  }'
```

## 🐛 Troubleshooting

### Erro: "Cannot open database"
- Verifique se o SQL Server LocalDB está instalado
- Execute: `sqllocaldb info` para listar instâncias
- Execute: `sqllocaldb start mssqllocaldb`

### Erro: "A network-related error occurred"
- Verifique a connection string no appsettings.json
- Verifique se o SQL Server está em execução

### Migrations não aplicadas
```powershell
# Ver status das migrations
dotnet ef migrations list

# Aplicar manualmente
dotnet ef database update
```

### Resetar banco de dados
```powershell
# Deletar banco
dotnet ef database drop

# Recriar com migrations
dotnet ef database update
```

## 💡 Dicas de Performance

1. **Use AsNoTracking para queries read-only:**
```csharp
var users = await _context.Users.AsNoTracking().ToListAsync();
```

2. **Carregue apenas os campos necessários:**
```csharp
var userNames = await _context.Users
    .Select(u => new { u.Id, u.FullName })
    .ToListAsync();
```

3. **Use paginação para grandes listas:**
```csharp
var notifications = await _context.Notifications
    .OrderByDescending(n => n.CreatedAt)
    .Skip(page * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

## 📖 Recursos Adicionais

- [Documentação do EF Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
