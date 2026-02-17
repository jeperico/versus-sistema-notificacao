using MinhaNotificacao.Entities;
using MinhaNotificacao.Exceptions;
using MinhaNotificacao.Repositories.Interfaces;
using MinhaNotificacao.Services.Interfaces;
using MinhaNotificacao.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MinhaNotificacao.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IUserRepository userRepository, ILogger<AccountService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserEntity> LoginAsync(LoginViewModel model)
    {
        var user = await _userRepository.GetByEmailAsync(model.Email);

        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            throw new UserInactiveException();
        }

        if (!VerifyPassword(model.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        // Atualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Usuário {Email} realizou login com sucesso", model.Email);

        return user;
    }

    public async Task<UserEntity> RegisterAsync(RegisterViewModel model)
    {
        // Verificar se o email já existe
        if (await _userRepository.ExistsAsync(u => u.Email == model.Email))
        {
            throw new UserAlreadyExistsException("email", model.Email);
        }

        // Verificar se o username já existe
        if (await _userRepository.ExistsAsync(u => u.Username == model.Username))
        {
            throw new UserAlreadyExistsException("nome de usuário", model.Username);
        }

        // Criar novo usuário
        var user = new UserEntity
        {
            Username = model.Username,
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = HashPassword(model.Password),
            Role = "NormalUser",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);

        _logger.LogInformation("Novo usuário registrado: {Email}", model.Email);

        return createdUser;
    }

    public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        return await Task.FromResult(new ClaimsPrincipal(identity));
    }

    public async Task<UserEntity?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == passwordHash;
    }
}
