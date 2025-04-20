using API_WebH3.DTO.Course;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class CourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    public CourseService(ICourseRepository courseRepository, IUserRepository userRepository)
{
    _courseRepository = courseRepository;
    _userRepository = userRepository;
}

public async Task<IEnumerable<CourseDto>> GetAllAsync()
{
    var courses = await _courseRepository.GetAllAsync();
    return courses.Select(c => new CourseDto
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        Price = c.Price,
        UrlImage = c.UrlImage,
        InstructorId = c.InstructorId,
        CategoryId = c.CategoryId,
        CreatedAt = c.CreatedAt,
        Contents = c.Contents
    });
}

public async Task<CourseDto> GetByIdAsync(Guid id)
{
    var course = await _courseRepository.GetByIdAsync(id);
    if (course == null)
    {
        return null;
    }

    return new CourseDto
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        Price = course.Price,
        UrlImage = course.UrlImage,
        InstructorId = course.InstructorId,
        CategoryId = course.CategoryId,
        CreatedAt = course.CreatedAt,
        Contents = course.Contents
    };
}

public async Task<CourseDto> CreateChapter(CreateCourseDto createCourseDto)
{
    var instructor = await _userRepository.GetByIdAsync(createCourseDto.InstructorId);
    if (instructor == null)
    {
        throw new ArgumentException("Instructor not found.");
    }
    
    var course = new Course 
    {
        Id = Guid.NewGuid(),
        Title = createCourseDto.Title,
        Description = createCourseDto.Description,
        Price = createCourseDto.Price,
        UrlImage = createCourseDto.UrlImage,
        InstructorId = createCourseDto.InstructorId,
        CategoryId = createCourseDto.CategoryId,
        CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
        Contents = createCourseDto.Contents,
        User = instructor
    };


    await _courseRepository.AddAsync(course);

    return new CourseDto
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        Price = course.Price,
        UrlImage = course.UrlImage,
        InstructorId = course.InstructorId,
        CategoryId = course.CategoryId,
        CreatedAt = course.CreatedAt,
        Contents = course.Contents
    };
}

public async Task<CourseDto> UpdateChapter(Guid id, UpdateCourseDto updateCourseDto)
{
    var course = await _courseRepository.GetByIdAsync(id);
    if (course == null)
    {
        return null;
    }

    course.Title = updateCourseDto.Title;
    course.Description = updateCourseDto.Description;
    course.Price = updateCourseDto.Price;
    course.UrlImage = updateCourseDto.UrlImage;
    course.InstructorId = updateCourseDto.InstructorId;
    course.CategoryId = updateCourseDto.CategoryId;
    course.Contents = updateCourseDto.Contents;

    await _courseRepository.UpdateAsync(course);

    return new CourseDto
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        Price = course.Price,
        UrlImage = course.UrlImage,
        InstructorId = course.InstructorId,
        CategoryId = course.CategoryId,
        CreatedAt = course.CreatedAt,
        Contents = course.Contents
    };
}

public async Task<bool> DeleteAsync(Guid id)
{
    var course = await _courseRepository.GetByIdAsync(id);
    if (course == null)
    {
        return false;
    }

    await _courseRepository.DeleteAsync(id);
    return true;
}
}