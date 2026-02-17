using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinhaNotificacao.Services.Interfaces;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Exceptions;
using MinhaNotificacao.Hubs;
using System.Security.Claims;

namespace MinhaNotificacao.Controllers;

[Authorize]
public class PokeController : Controller
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<PokeNotificationHub> _hubContext;
    private readonly ILogger<PokeController> _logger;

    public PokeController(
        IUserService userService, 
        INotificationService notificationService,
        IHubContext<PokeNotificationHub> hubContext,
        ILogger<PokeController> logger)
    {
        _userService = userService;
        _notificationService = notificationService;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            var allUsers = await _userService.GetAllUsersAsync();
            
            // Filtrar para não mostrar o próprio usuário
            var otherUsers = allUsers.Where(u => u.Id != currentUserId).ToList();
            
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.CurrentUserName = User.Identity?.Name;
            ViewBag.CurrentUserRole = currentUserRole;
            ViewBag.IsAdmin = currentUserRole == "Admin";
            
            return View(otherUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar lista de usuários para poke");
            TempData["ErrorMessage"] = "Erro ao carregar usuários. Tente novamente.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    [Route("api/poke/{userId}")]
    public async Task<IActionResult> PokeUser(int userId)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserName = User.FindFirstValue(ClaimTypes.GivenName) ?? User.Identity?.Name ?? "Alguém";
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Verificar se o usuário atual é Admin
            if (currentUserRole != "Admin")
            {
                return Forbid();
            }

            // Verificar se não está tentando cutucar a si mesmo
            if (currentUserId == userId)
            {
                return BadRequest(new { success = false, message = "Você não pode cutucar a si mesmo!" });
            }

            // Verificar se o usuário existe
            var targetUser = await _userService.GetUserByIdAsync(userId);
            if (targetUser == null)
            {
                return NotFound(new { success = false, message = "Usuário não encontrado." });
            }

            // Verificar se o usuário está ativo
            if (!targetUser.IsActive)
            {
                return BadRequest(new { success = false, message = "Não é possível cutucar um usuário inativo." });
            }

            // Criar notificação de cutucada
            var notification = new NotificationEntity
            {
                UserId = userId,
                Title = "👋 Você foi cutucado!",
                Message = $"{currentUserName} cutucou você! Que tal cutucar de volta?",
                Type = "Info",
                Link = "/Poke",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationService.CreateNotificationAsync(notification);

            _logger.LogInformation("Usuário {CurrentUserId} cutucou usuário {TargetUserId}", currentUserId, userId);

            // Enviar notificação em tempo real via SignalR
            await _hubContext.Clients.Group($"User-{userId}").SendAsync("ReceivePoke", new
            {
                fromUserId = currentUserId,
                fromUserName = currentUserName,
                message = $"{currentUserName} cutucou você!",
                timestamp = DateTime.UtcNow,
                notificationId = notification.Id
            });

            return Ok(new 
            { 
                success = true, 
                message = $"Você cutucou {targetUser.FullName}!",
                userName = targetUser.FullName,
                isOnline = PokeNotificationHub.IsUserOnline(userId)
            });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar cutucada");
            return StatusCode(500, new { success = false, message = "Erro ao processar cutucada. Tente novamente." });
        }
    }

    [HttpGet]
    [Route("api/poke/stats")]
    public async Task<IActionResult> GetPokeStats()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Contar quantas cutucadas este usuário recebeu
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(currentUserId);
            var pokeCount = notifications.Count(n => n.Title.Contains("cutucado"));

            return Ok(new { success = true, pokeCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas de poke");
            return StatusCode(500, new { success = false, message = "Erro ao obter estatísticas." });
        }
    }

    [HttpPost]
    [Route("api/poke/all")]
    public async Task<IActionResult> PokeAll()
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserName = User.FindFirstValue(ClaimTypes.GivenName) ?? User.Identity?.Name ?? "Alguém";
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Verificar se o usuário atual é Admin
            if (currentUserRole != "Admin")
            {
                return Forbid();
            }

            // Buscar todos os usuários exceto o próprio
            var allUsers = await _userService.GetAllUsersAsync();
            var targetUsers = allUsers.Where(u => u.Id != currentUserId && u.IsActive).ToList();

            if (!targetUsers.Any())
            {
                return BadRequest(new { success = false, message = "Não há outros usuários para cutucar." });
            }

            var successCount = 0;
            var failureCount = 0;

            // Cutucar cada usuário
            foreach (var user in targetUsers)
            {
                try
                {
                    // Criar notificação de cutucada
                    var notification = new NotificationEntity
                    {
                        UserId = user.Id,
                        Title = "👋 Você foi cutucado!",
                        Message = $"{currentUserName} cutucou você! Que tal cutucar de volta?",
                        Type = "Info",
                        Link = "/Poke",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _notificationService.CreateNotificationAsync(notification);

                    // Enviar notificação em tempo real via SignalR
                    await _hubContext.Clients.Group($"User-{user.Id}").SendAsync("ReceivePoke", new
                    {
                        fromUserId = currentUserId,
                        fromUserName = currentUserName,
                        message = $"{currentUserName} cutucou você!",
                        timestamp = DateTime.UtcNow,
                        notificationId = notification.Id
                    });

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao cutucar usuário {UserId}", user.Id);
                    failureCount++;
                }
            }

            _logger.LogInformation("Usuário {CurrentUserId} cutucou {SuccessCount} usuários (falhas: {FailureCount})", 
                currentUserId, successCount, failureCount);

            return Ok(new 
            { 
                success = true, 
                message = $"Você cutucou {successCount} usuário(s)!",
                successCount,
                failureCount,
                totalUsers = targetUsers.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar cutucada em massa");
            return StatusCode(500, new { success = false, message = "Erro ao processar cutucadas. Tente novamente." });
        }
    }
}
