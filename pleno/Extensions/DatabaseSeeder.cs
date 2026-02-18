using MinhaNotificacao.Data;
using MinhaNotificacao.Entities;

namespace MinhaNotificacao.Extensions;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verifica se já existem dados
        if (context.Users.Any())
            return;

        // Criar usuários de exemplo
        var users = new List<UserEntity>
        {
            new UserEntity
            {
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "i168bkEMCJarPr74qFObWf0qb2pK3+LZt7zf6LbHLs0=", // Em produção, use um hash real
                FullName = "Administrador do Sistema",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}
