using MinhaNotificacao.Entities;
using MinhaNotificacao.Exceptions;
using MinhaNotificacao.Repositories.Interfaces;
using MinhaNotificacao.Services.Interfaces;

namespace MinhaNotificacao.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async Task<NotificationEntity?> GetNotificationByIdAsync(int id)
    {
        return await _notificationRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<NotificationEntity>> GetAllNotificationsAsync()
    {
        return await _notificationRepository.GetAllAsync();
    }

    public async Task<IEnumerable<NotificationEntity>> GetNotificationsByUserIdAsync(int userId)
    {
        return await _notificationRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<NotificationEntity>> GetUnreadNotificationsByUserIdAsync(int userId)
    {
        return await _notificationRepository.GetUnreadByUserIdAsync(userId);
    }

    public async Task<IEnumerable<NotificationEntity>> GetNotificationsByTypeAsync(string type)
    {
        return await _notificationRepository.GetByTypeAsync(type);
    }

    public async Task<NotificationEntity> CreateNotificationAsync(NotificationEntity notification)
    {
        // Validação: verifica se o usuário existe
        var userExists = await _userRepository.ExistsAsync(u => u.Id == notification.UserId);
        if (!userExists)
        {
            throw new EntityNotFoundException("Usuário", notification.UserId);
        }

        // Validação do tipo
        var validTypes = new[] { "Info", "Warning", "Error", "Success" };
        if (!validTypes.Contains(notification.Type))
        {
            throw new ValidationException("Tipo", $"Tipo de notificação inválido: {notification.Type}. Use: Info, Warning, Error ou Success");
        }

        notification.CreatedAt = DateTime.UtcNow;
        return await _notificationRepository.AddAsync(notification);
    }

    public async Task UpdateNotificationAsync(NotificationEntity notification)
    {
        var existingNotification = await _notificationRepository.GetByIdAsync(notification.Id);
        if (existingNotification == null)
        {
            throw new EntityNotFoundException("Notificação", notification.Id);
        }

        // Validação do tipo
        var validTypes = new[] { "Info", "Warning", "Error", "Success" };
        if (!validTypes.Contains(notification.Type))
        {
            throw new ValidationException("Tipo", $"Tipo de notificação inválido: {notification.Type}. Use: Info, Warning, Error ou Success");
        }

        await _notificationRepository.UpdateAsync(notification);
    }

    public async Task DeleteNotificationAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
        {
            throw new EntityNotFoundException("Notificação", id);
        }

        await _notificationRepository.DeleteAsync(notification);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadByUserIdAsync(int userId)
    {
        await _notificationRepository.MarkAllAsReadByUserIdAsync(userId);
    }

    public async Task<int> GetUnreadCountByUserIdAsync(int userId)
    {
        return await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
    }

    public async Task<int> GetTotalNotificationsCountAsync()
    {
        return await _notificationRepository.CountAsync();
    }
}
