using Microsoft.EntityFrameworkCore;
using MinhaNotificacao.Data;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Repositories.Interfaces;

namespace MinhaNotificacao.Repositories;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UserEntity?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<UserEntity>> GetActiveUsersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string role)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.Role == role)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public override async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
