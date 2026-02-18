using Microsoft.EntityFrameworkCore;
using MinhaNotificacao.Data;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Repositories.Interfaces;

namespace MinhaNotificacao.Repositories;

public class NotificationRepository : Repository<NotificationEntity>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NotificationEntity>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NotificationEntity>> GetUnreadByUserIdAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<NotificationEntity>> GetByTypeAsync(string type)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(n => n.Type == type)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountByUserIdAsync(int userId)
    {
        return await _dbSet
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _dbSet.FindAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadByUserIdAsync(int userId)
    {
        var notifications = await _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public override async Task<NotificationEntity?> GetByIdAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);
    }
}
