using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly AppDbContext _context;

    public CouponRepository(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<Coupon>> GetAllAsync()
    {
       return await _context.Coupons.ToListAsync();
    }

    public async Task<Coupon> GetByIdAsync(Guid id)
    {
        return await _context.Coupons.FindAsync(id);
    }

    public async Task<Coupon> GetByCodeAsync(string code)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async  Task AddAsync(Coupon coupon)
    {
        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Coupon coupon)
    {
        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
      var coupon = await _context.Coupons.FindAsync(id);
      if (coupon != null)
      {
          _context.Coupons.Remove(coupon);
          await _context.SaveChangesAsync();
      }
    }
}