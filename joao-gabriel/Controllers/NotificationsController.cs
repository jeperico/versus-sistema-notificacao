using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Services.Interfaces;
using System.Security.Claims;

namespace MinhaNotificacao.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationEntity>>> GetAll()
    {
        var notifications = await _notificationService.GetAllNotificationsAsync();
        return Ok(notifications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationEntity>> GetById(int id)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id);
        if (notification == null)
            return NotFound($"Notificação com ID {id} não encontrada");

        return Ok(notification);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationEntity>>> GetByUserId(int userId)
    {
        var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("user/{userId}/unread")]
    public async Task<ActionResult<IEnumerable<NotificationEntity>>> GetUnreadByUserId(int userId)
    {
        var notifications = await _notificationService.GetUnreadNotificationsByUserIdAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("user/{userId}/unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(int userId)
    {
        var count = await _notificationService.GetUnreadCountByUserIdAsync(userId);
        return Ok(count);
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<NotificationEntity>>> GetByType(string type)
    {
        var notifications = await _notificationService.GetNotificationsByTypeAsync(type);
        return Ok(notifications);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationEntity>> Create([FromBody] NotificationEntity notification)
    {
        try
        {
            var createdNotification = await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = createdNotification.Id }, createdNotification);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] NotificationEntity notification)
    {
        if (id != notification.Id)
            return BadRequest("ID da URL não corresponde ao ID da notificação");

        try
        {
            await _notificationService.UpdateNotificationAsync(notification);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{id}/mark-as-read")]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("user/{userId}/mark-all-as-read")]
    public async Task<ActionResult> MarkAllAsRead(int userId)
    {
        await _notificationService.MarkAllAsReadByUserIdAsync(userId);
        return NoContent();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        var count = await _notificationService.GetTotalNotificationsCountAsync();
        return Ok(count);
    }

    // Endpoints para o usuário logado
    [Authorize]
    [HttpGet("my/test")]
    public ActionResult TestAuth()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name;
        return Ok(new { 
            authenticated = User.Identity?.IsAuthenticated ?? false,
            userId = userId,
            userName = userName,
            claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    [Authorize]
    [HttpGet("my/unread-count")]
    public async Task<ActionResult<int>> GetMyUnreadCount()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await _notificationService.GetUnreadCountByUserIdAsync(userId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao carregar contagem", details = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("my/recent")]
    public async Task<ActionResult<IEnumerable<NotificationEntity>>> GetMyRecentNotifications([FromQuery] int limit = 10)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
            var recent = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToList();
            return Ok(recent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao carregar notificações", details = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("mark-read")]
    public async Task<ActionResult> MarkMultipleAsRead([FromBody] int[] notificationIds)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        try
        {
            foreach (var id in notificationIds)
            {
                var notification = await _notificationService.GetNotificationByIdAsync(id);
                if (notification != null && notification.UserId == userId)
                {
                    await _notificationService.MarkAsReadAsync(id);
                }
            }
            return Ok(new { success = true, message = "Notificações marcadas como lidas" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("my/mark-all-read")]
    public async Task<ActionResult> MarkAllMyNotificationsAsRead()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _notificationService.MarkAllAsReadByUserIdAsync(userId);
        return Ok(new { success = true, message = "Todas as notificações foram marcadas como lidas" });
    }
}
