using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<Chat>> GetAllAsync()
    {
        return await _context.Chats.Include(c=>c.Messages).ToListAsync();
    }

    public async Task<Chat> GetByIdAsync(Guid id)
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Chat> GetByUsersAsync(Guid user1Id, Guid user2Id)
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c =>
                (c.User1Id == user1Id && c.User2Id == user2Id) ||
                (c.User1Id == user2Id && c.User2Id == user1Id));
    }

    public async Task<IEnumerable<Chat>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Chats
            .Include(c => c.Messages)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .ToListAsync();
    }

    public async  Task AddAsync(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (chat.Messages.Any())
        {
            _context.Messages.RemoveRange(chat.Messages);
        }

        if (chat!=null)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
        }
    }
}