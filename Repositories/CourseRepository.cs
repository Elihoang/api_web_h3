using API_WebH3.Data;
using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebH3.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CourseRepository(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IEnumerable<Course>> SearchCoursesAsync(string keyword, int page, int pageSize)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(keyword) ||
                                     (c.Description != null && c.Description.ToLower().Contains(keyword)));
        }

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(Guid id)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddCourseAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCourseAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string?> UploadImageAsync(Guid courseId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return null;

        // Tạo thư mục uploads nếu chưa có
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // Tạo tên file duy nhất
        string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Lưu file vào thư mục
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Cập nhật đường dẫn ảnh trong database
        course.UrlImage = $"/uploads/{uniqueFileName}";
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();

        return course.UrlImage;
    }
}
