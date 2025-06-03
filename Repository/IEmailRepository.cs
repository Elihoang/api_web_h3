using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface IEmailRepository
{
    Task AddEmailAsync(Email email);
    Task<List<Email>> GetAllEmailsAsync();
    Task<Email> GetEmailByIdAsync(int id);
    Task<List<Email>> GetEmailsBySourceTypeAsync(string sourceType);
}