using MinhaNotificacao.Entities;

namespace MinhaNotificacao.Repositories.Interfaces;

public interface INotificationRepository : IRepository<NotificationEntity>
{
    Task<IEnumerable<NotificationEntity>> GetByUserIdAsync(int userId);
    Task<IEnumerable<NotificationEntity>> GetUnreadByUserIdAsync(int userId);
    Task<IEnumerable<NotificationEntity>> GetByTypeAsync(string type);
    Task<int> GetUnreadCountByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadByUserIdAsync(int userId);
}
