using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .Include(n => n.UserNotifications)
            .ToListAsync();
    }

    public async Task<Notification> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
            .Include(n => n.UserNotifications)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notification = await _context.Notifications
            .Include(n => n.UserNotifications)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
        if (notification.UserNotifications.Any())
        {
            _context.UserNotifications.RemoveRange(notification.UserNotifications);
        }
    }
    public async Task UpdateUserNotificationAsync(UserNotification userNotification)
    {
        _context.UserNotifications.Update(userNotification);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Include(n => n.UserNotifications)
            .Where(n => n.UserNotifications.Any(un => un.UserId == userId))
            .ToListAsync();
    }
}