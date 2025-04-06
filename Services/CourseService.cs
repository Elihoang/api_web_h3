using API_WebH3.Data;
using API_WebH3.DTOs.Course;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class CourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public CourseService(ICourseRepository courseRepository, IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<CourseDto>> SearchCoursesAsync(
        string keyword,
        string category,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize)
    {
        // Gọi phương thức tìm kiếm từ repository
        var courses = await _courseRepository.SearchCoursesAsync(keyword, page, pageSize);

        // Ánh xạ sang DTO
        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Price = c.Price,
            InstructorId = c.InstructorId,
            UrlImage = c.UrlImage,
            CreatedAt = c.CreatedAt,
            Contents = c.Contents
        }).ToList();
    }
    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
    
        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Price = c.Price,
            InstructorId = c.InstructorId,
            UrlImage = c.UrlImage,
            CreatedAt = c.CreatedAt,
            Contents = c.Contents
        }).ToList();
    }
    
    public async Task<CourseDto> AddCourseAsync(CreateCourseDto courseDto)
    {
        var instructorExists = await _userRepository.ExistsAsync(courseDto.InstructorId);
        if (!instructorExists)
        {
            throw new Exception("InstructorId không hợp lệ: Người hướng dẫn không tồn tại!");
        }
        
        var course = new Course
        {
            Id = Guid.NewGuid(), // Tạo ID mới
            Title = courseDto.Title,
            Description = courseDto.Description,
            Price = courseDto.Price,
            InstructorId = courseDto.InstructorId,
            Contents = courseDto.Contents
        };

        await _courseRepository.AddCourseAsync(course);

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            InstructorId = course.InstructorId,
            CreatedAt = course.CreatedAt
        };
    }
    
    public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null) return null;

        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            UrlImage = course.UrlImage,
            InstructorId = course.InstructorId,
            CreatedAt = course.CreatedAt,
            Contents = course.Contents
        };
    }
    
    public async Task UpdateCourseAsync(Guid id, CreateCourseDto courseDto)
    {
        var existingCourse = await _courseRepository.GetCourseByIdAsync(id);
        if (existingCourse == null) throw new Exception("UPDATE: Không tìm thấy khóa học!");

        existingCourse.Title = courseDto.Title;
        existingCourse.Description = courseDto.Description;
        existingCourse.Price = courseDto.Price;
        existingCourse.Contents = courseDto.Contents;
        existingCourse.InstructorId = courseDto.InstructorId;

        await _courseRepository.UpdateCourseAsync(existingCourse);
    }
    
    public async Task DeleteCourseAsync(Guid id)
    {
        await _courseRepository.DeleteCourseAsync(id);
    }

    public async Task<string?> UploadImageAsync(Guid courseId, IFormFile file)
    {
        return await _courseRepository.UploadImageAsync(courseId, file);
    }
}