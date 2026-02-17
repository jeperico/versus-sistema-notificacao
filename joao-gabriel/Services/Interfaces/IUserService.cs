using MinhaNotificacao.Entities;

namespace MinhaNotificacao.Services.Interfaces;

public interface IUserService
{
    Task<UserEntity?> GetUserByIdAsync(int id);
    Task<UserEntity?> GetUserByEmailAsync(string email);
    Task<UserEntity?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    Task<IEnumerable<UserEntity>> GetActiveUsersAsync();
    Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string role);
    Task<UserEntity> CreateUserAsync(UserEntity user);
    Task UpdateUserAsync(UserEntity user);
    Task DeleteUserAsync(int id);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<bool> UserExistsByUsernameAsync(string username);
    Task UpdateLastLoginAsync(int userId);
    Task<int> GetTotalUsersCountAsync();
}
