using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class UserNotificationRepository : IUserNotificationRepository
{
    private readonly AppDbContext _context;

    public UserNotificationRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<UserNotification>> GetAllAsync()
    {
        return await _context.UserNotifications
            .Include(un => un.Notification)
            .ToListAsync(); 
    }

    public async Task<UserNotification> GetByIdAsync(Guid id)
    {
        return await _context.UserNotifications
            .Include(un => un.Notification)
            .FirstOrDefaultAsync(un => un.Id == id);
    }

    public async Task<UserNotification> GetByUserAndNotificationAsync(Guid userId, Guid notificationId)
    {
        return await _context.UserNotifications
            .Include(un => un.Notification)
            .FirstOrDefaultAsync(un => un.UserId == userId && un.NotificationId == notificationId);
    }

    public async Task<IEnumerable<UserNotification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserNotifications
            .Include(un => un.Notification)
            .Where(un => un.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(UserNotification userNotification)
    {
        await _context.UserNotifications.AddAsync(userNotification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserNotification userNotification)
    {
        _context.UserNotifications.Update(userNotification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var userNotification = await _context.UserNotifications.FindAsync(id);
        if (userNotification !=null)
        {
            _context.UserNotifications.Remove(userNotification);
            await _context.SaveChangesAsync();
        }
        
    }
}