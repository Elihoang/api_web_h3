using API_WebH3.Models;

namespace API_WebH3.Repository;

public interface ICouponRepository
{
    Task<IEnumerable<Coupon>> GetAllAsync();
    Task<Coupon> GetByIdAsync(Guid id);
    Task<Coupon> GetByCodeAsync(string code);
    Task AddAsync(Coupon coupon);
    Task UpdateAsync(Coupon coupon);
    Task DeleteAsync(Guid id);
}