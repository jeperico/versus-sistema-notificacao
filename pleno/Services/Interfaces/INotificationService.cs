using MinhaNotificacao.Entities;

namespace MinhaNotificacao.Services.Interfaces;

public interface INotificationService
{
    Task<NotificationEntity?> GetNotificationByIdAsync(int id);
    Task<IEnumerable<NotificationEntity>> GetAllNotificationsAsync();
    Task<IEnumerable<NotificationEntity>> GetNotificationsByUserIdAsync(int userId);
    Task<IEnumerable<NotificationEntity>> GetUnreadNotificationsByUserIdAsync(int userId);
    Task<IEnumerable<NotificationEntity>> GetNotificationsByTypeAsync(string type);
    Task<NotificationEntity> CreateNotificationAsync(NotificationEntity notification);
    Task UpdateNotificationAsync(NotificationEntity notification);
    Task DeleteNotificationAsync(int id);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadByUserIdAsync(int userId);
    Task<int> GetUnreadCountByUserIdAsync(int userId);
    Task<int> GetTotalNotificationsCountAsync();
}
