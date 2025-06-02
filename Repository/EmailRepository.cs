using API_WebH3.Data;
using API_WebH3.Models;

namespace API_WebH3.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly AppDbContext _context;

    public EmailRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddEmailAsync(Email email)
    {
        _context.Emails.Add(email);
        await _context.SaveChangesAsync();
    }
}