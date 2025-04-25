using API_WebH3.DTO.Coupon;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class CouponService
{
    private readonly ICouponRepository _couponRepository;

    public CouponService(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }
    
    public async Task<IEnumerable<CouponDto>> GetAllAsync()
    {
        var coupons = await _couponRepository.GetAllAsync();
        return coupons.Select(c => new CouponDto
        {
            Id = c.Id,
            Code = c.Code,
            DiscountPercentage = c.DiscountPercentage,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            MaxUsage = c.MaxUsage,
            CurrentUsage = c.CurrentUsage,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        });
    }
    
    public async Task<CouponDto> GetByIdAsync(Guid id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);
        if (coupon == null)
        {
            return null;
        }
        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            MaxUsage = coupon.MaxUsage,
            CurrentUsage = coupon.CurrentUsage,
            IsActive = coupon.IsActive,
            CreatedAt = coupon.CreatedAt
        };
    }
    
    public async Task<CouponDto> GetByCodeAsync(string code)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code);
        if (coupon == null)
        {
            return null;
        }
        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            MaxUsage = coupon.MaxUsage,
            CurrentUsage = coupon.CurrentUsage,
            IsActive = coupon.IsActive,
            CreatedAt = coupon.CreatedAt
        };
    }
    
    public async Task<CouponDto> CreateAsync(CreateCouponDto createCouponDto)
    {
        var existingCoupon = await _couponRepository.GetByCodeAsync(createCouponDto.Code);
        if (existingCoupon != null)
        {
            throw new ArgumentException("Coupon code already exists.");
        }
        
        if (createCouponDto.DiscountPercentage < 0 || createCouponDto.DiscountPercentage > 100)
        {
            throw new ArgumentException("Discount percentage must be between 0 and 100.");
        }
        
        if (createCouponDto.StartDate >= createCouponDto.EndDate)
        {
            throw new ArgumentException("Start date must be earlier than end date.");
        }
        
        if (createCouponDto.MaxUsage <= 0)
        {
            throw new ArgumentException("Max usage must be greater than 0.");
        }

        var coupon = new Coupon
        {
            Id = Guid.NewGuid(),
            Code = createCouponDto.Code,
            DiscountPercentage = createCouponDto.DiscountPercentage,
            StartDate = createCouponDto.StartDate.ToUniversalTime(),
            EndDate = createCouponDto.EndDate.ToUniversalTime(),
            MaxUsage = createCouponDto.MaxUsage,
            CurrentUsage = 0,
            IsActive = createCouponDto.IsActive,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _couponRepository.AddAsync(coupon);

        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            MaxUsage = coupon.MaxUsage,
            CurrentUsage = coupon.CurrentUsage,
            IsActive = coupon.IsActive,
            CreatedAt = coupon.CreatedAt
        };
    }
    
    public async Task<CouponDto> UpdateAsync(Guid id, UpdateCouponDto updateCouponDto)
    {
    var coupon = await _couponRepository.GetByIdAsync(id);
    if (coupon == null)
    {
        return null;
    }
    

    var existingCoupon = await _couponRepository.GetByCodeAsync(updateCouponDto.Code);
    if (existingCoupon != null && existingCoupon.Id != id)
    {
        throw new ArgumentException("Coupon code already exists.");
    }
    
    if (updateCouponDto.DiscountPercentage < 0 || updateCouponDto.DiscountPercentage > 100)
    {
        throw new ArgumentException("Discount percentage must be between 0 and 100.");
    }
    
    if (updateCouponDto.StartDate >= updateCouponDto.EndDate)
    {
        throw new ArgumentException("Start date must be earlier than end date.");
    }
    if (updateCouponDto.MaxUsage <= 0)
    {
        throw new ArgumentException("Max usage must be greater than 0.");
    }
    if (updateCouponDto.MaxUsage < coupon.CurrentUsage)
    {
        throw new ArgumentException("Max usage cannot be less than current usage.");
    }
    
    coupon.Code = updateCouponDto.Code;
    coupon.DiscountPercentage = updateCouponDto.DiscountPercentage;
    coupon.StartDate = updateCouponDto.StartDate.ToUniversalTime();
    coupon.EndDate = updateCouponDto.EndDate.ToUniversalTime();
    coupon.MaxUsage = updateCouponDto.MaxUsage;
    coupon.IsActive = updateCouponDto.IsActive;
    
    await _couponRepository.UpdateAsync(coupon);
    
    return new CouponDto
    {
    Id = coupon.Id,
    Code = coupon.Code,
    DiscountPercentage = coupon.DiscountPercentage,
    StartDate = coupon.StartDate,
    EndDate = coupon.EndDate,
    MaxUsage = coupon.MaxUsage,
    CurrentUsage = coupon.CurrentUsage,
    IsActive = coupon.IsActive,
    CreatedAt = coupon.CreatedAt
    };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var coupon = await _couponRepository.GetByIdAsync(id);
        if (coupon == null)
        {
            return false;
        }
        await _couponRepository.DeleteAsync(id);
        return true;
    }
}