using MinhaNotificacao.Entities;

namespace MinhaNotificacao.Repositories.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<IEnumerable<UserEntity>> GetActiveUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string role);
}
