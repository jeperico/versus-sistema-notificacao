using MinhaNotificacao.Entities;
using MinhaNotificacao.Repositories.Interfaces;
using MinhaNotificacao.Services.Interfaces;

namespace MinhaNotificacao.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<UserEntity?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<IEnumerable<UserEntity>> GetActiveUsersAsync()
    {
        return await _userRepository.GetActiveUsersAsync();
    }

    public async Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string role)
    {
        return await _userRepository.GetUsersByRoleAsync(role);
    }

    public async Task<UserEntity> CreateUserAsync(UserEntity user)
    {
        // Validações
        if (await UserExistsByEmailAsync(user.Email))
        {
            throw new InvalidOperationException($"Já existe um usuário com o email {user.Email}");
        }

        if (await UserExistsByUsernameAsync(user.Username))
        {
            throw new InvalidOperationException($"Já existe um usuário com o username {user.Username}");
        }

        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        return await _userRepository.AddAsync(user);
    }

    public async Task UpdateUserAsync(UserEntity user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"Usuário com ID {user.Id} não encontrado");
        }

        // Verifica se o email está sendo alterado para um já existente
        if (existingUser.Email != user.Email && await UserExistsByEmailAsync(user.Email))
        {
            throw new InvalidOperationException($"Já existe um usuário com o email {user.Email}");
        }

        // Verifica se o username está sendo alterado para um já existente
        if (existingUser.Username != user.Username && await UserExistsByUsernameAsync(user.Username))
        {
            throw new InvalidOperationException($"Já existe um usuário com o username {user.Username}");
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"Usuário com ID {id} não encontrado");
        }

        await _userRepository.DeleteAsync(user);
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _userRepository.ExistsAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistsByUsernameAsync(string username)
    {
        return await _userRepository.ExistsAsync(u => u.Username == username);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await _userRepository.CountAsync();
    }
}
