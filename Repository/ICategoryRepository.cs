using System.Collections;
using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ICategoryRepository
{
   Task<IEnumerable<Category>> GetAllAsync();
   Task<Category> GetByIdAsync(string id);
   Task AddAsync(Category category);
   Task UpdateAsync(Category category);
   Task DeleteAsync(string id);
    
}