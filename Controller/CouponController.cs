using API_WebH3.DTO.Coupon;
using API_WebH3.Service;
using Microsoft.AspNetCore.Mvc;

namespace API_WebH3.Controller;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly CouponService _couponService;

    public CouponController(CouponService couponService)
    {
        _couponService = couponService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CouponDto>>> GetCoupons()
    {
        var coupons = await _couponService.GetAllAsync();
        return Ok(coupons);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CouponDto>> GetCoupon(Guid id)
    {
        var coupon = await _couponService.GetByIdAsync(id);
        if (coupon == null)
        {
            return NotFound();
        }
        return Ok(coupon);
    }
    
    [HttpGet("code/{code}")]
    public async Task<ActionResult<CouponDto>> GetCouponByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { message = "Mã coupon không được để trống." });
        }

        var coupon = await _couponService.GetByCodeAsync(code);
        if (coupon == null)
        {
            return NotFound(new { message = $"Mã coupon {code} không tồn tại." });
        }
        return Ok(coupon);
    }
    
    [HttpPost]
    public async Task<ActionResult<CouponDto>> CreateCoupon(CreateCouponDto createCouponDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var couponDto = await _couponService.CreateAsync(createCouponDto);
        return CreatedAtAction(nameof(GetCoupon), new { id = couponDto.Id }, couponDto);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(Guid id, UpdateCouponDto updateCouponDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var couponDto = await _couponService.UpdateAsync(id, updateCouponDto);
        if (couponDto == null)
        {
                return NotFound();
        }
        return Ok(couponDto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCoupon(Guid id)
    {
        var coupon = await _couponService.DeleteAsync(id);
        if (!coupon)
        {
            return NotFound();
        }
        return NoContent();
    }
}