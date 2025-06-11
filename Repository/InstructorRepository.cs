using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class InstructorRepository : IInstructorRepository
{
    private readonly AppDbContext _context;

    public InstructorRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.Where(u => u.Role == "Instructor").ToListAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id && u.Role == "Instructor");
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Role = "Instructor"; 
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.Role = "Instructor"; // Đảm bảo Role không bị thay đổi
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}