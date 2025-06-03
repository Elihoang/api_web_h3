using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

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
    public async Task<List<Email>> GetAllEmailsAsync()
    {
        return await _context.Emails
            .OrderByDescending(e => e.SentAt)
            .ToListAsync();
    }

    public async Task<Email> GetEmailByIdAsync(int id)
    {
        return await _context.Emails.FindAsync(id);
    }

    public async Task<List<Email>> GetEmailsBySourceTypeAsync(string sourceType)
    {
        return await _context.Emails
            .Where(e => e.SourceType == sourceType)
            .OrderByDescending(e => e.SentAt)
            .ToListAsync();
    }
}