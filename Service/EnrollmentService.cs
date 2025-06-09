using API_WebH3.DTO.Enrollment;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class EnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository; 
    private readonly IUserRepository _userRepository; 
    private readonly ICourseRepository _courseRepository;
    
    public EnrollmentService(IEnrollmentRepository enrollmentRepository, IUserRepository userRepository, ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }
    public async Task<IEnumerable<EnrollmentDto>> GetAllAsync()
    {
        var enrollments = await _enrollmentRepository.GetAllAsync();
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            CreatedAt = e.CreatedAt
        });
    }
    public async Task<EnrollmentDto> GetByIdAsync(int id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return null;
        }
        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status,
            CreatedAt = enrollment.CreatedAt
        };
    }
    public async Task<IEnumerable<EnrollmentDto>> GetByUserIdAsync(Guid userId)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            CreatedAt = e.CreatedAt
        });
    }
    public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(string courseId)
    {
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(courseId);
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status,
            CreatedAt = e.CreatedAt
        });
    }
    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto createEnrollmentDto)
    {
        var user = await _userRepository.GetByIdAsync(createEnrollmentDto.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var course = await _courseRepository.GetByIdAsync(createEnrollmentDto.CourseId);
        if (course == null)
        {
            throw new ArgumentException("Course not found.");
        }

        var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(createEnrollmentDto.UserId, createEnrollmentDto.CourseId);
        if (existingEnrollment != null)
        {
            throw new ArgumentException("User is already enrolled in this course.");
        }

        var enrollment = new Enrollment
        {
            UserId = createEnrollmentDto.UserId,
            CourseId = createEnrollmentDto.CourseId,
            EnrolledAt = DateTime.UtcNow,
            Status = createEnrollmentDto.Status ?? "Enrolled",
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _enrollmentRepository.AddAsync(enrollment);

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status,
            CreatedAt = enrollment.CreatedAt
        };
    }
    public async Task<EnrollmentDto> UpdateAsync(int id, UpdateEnrollmentDto updateEnrollmentDto)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return null;
        }

        if (!new[] { "Enrolled", "Completed", "Failed" }.Contains(updateEnrollmentDto.Status))
        {
            throw new ArgumentException("Status must be 'Enrolled', 'Completed', or 'Failed'.");
        }

        enrollment.Status = updateEnrollmentDto.Status;

        await _enrollmentRepository.UpdateAsync(enrollment);

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status,
            CreatedAt = enrollment.CreatedAt
        };
    }
    public async Task<EnrollmentDto> GetByUserAndCourseAsync(Guid userId, string courseId)
    {
        var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);
        if (enrollment == null)
        {
            return null;
        }

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status
        };
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return false;
        }
        await _enrollmentRepository.DeleteAsync(id);
        return true;
    }
}