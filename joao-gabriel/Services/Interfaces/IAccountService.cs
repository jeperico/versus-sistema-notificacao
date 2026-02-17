using MinhaNotificacao.Entities;
using MinhaNotificacao.ViewModels;
using System.Security.Claims;

namespace MinhaNotificacao.Services.Interfaces;

public interface IAccountService
{
    Task<UserEntity> LoginAsync(LoginViewModel model);
    Task<UserEntity> RegisterAsync(RegisterViewModel model);
    Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(UserEntity user);
    Task<UserEntity?> GetUserByEmailAsync(string email);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}
