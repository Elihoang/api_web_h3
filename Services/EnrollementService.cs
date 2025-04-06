using API_WebH3.DTOs.Enrollment;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class EnrollementService
{
    private readonly IEnrollementRepository _enrollementRepository;

    public EnrollementService(IEnrollementRepository enrollementRepository)
    {
        _enrollementRepository = enrollementRepository;
    }

    public async Task<List<EnrollmentDto>> GetAllAsync()
    {
        var enrollments = await _enrollementRepository.GetAllAsync();
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status
        }).ToList();
    }

    public async Task<EnrollmentDto> GetByIdAsync(int id)
    {
        var enrollment = await _enrollementRepository.GetByIdAsync(id);
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

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto createEnrollmenDto)
    {
        var enrollment = new Enrollment
        {
            UserId = createEnrollmenDto.UserId,
            CourseId = createEnrollmenDto.CourseId,
            EnrolledAt = DateTime.UtcNow,
            Status = createEnrollmenDto.Status ?? "Active"
            
        };
        var createEnrollment=await _enrollementRepository.CreateAsync(enrollment);
        return new EnrollmentDto
        {
            Id = createEnrollment.Id,
            UserId = createEnrollment.UserId,
            CourseId = createEnrollment.CourseId,
            EnrolledAt = createEnrollment.EnrolledAt,
            Status = createEnrollment.Status
           
        };
        
    }

    public async Task<EnrollmentDto> UpdateAsync(int id , CreateEnrollmentDto updateEnrollmentDto)
    {
        var enrollment = await _enrollementRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            return null;
        }
        enrollment.UserId = updateEnrollmentDto.UserId;
        enrollment.CourseId = updateEnrollmentDto.CourseId;
        enrollment.Status = updateEnrollmentDto.Status ?? enrollment.Status;
        var updateEnrollment = await _enrollementRepository.UpdateAsync(enrollment);

        return new EnrollmentDto
        {
            Id = updateEnrollment.Id,
            UserId = updateEnrollment.UserId,
            CourseId = updateEnrollment.CourseId,
            EnrolledAt = updateEnrollment.EnrolledAt,
            Status = updateEnrollment.Status

        };
    }
    public async Task<bool> DeleteAsync(int id)
    {
        return await _enrollementRepository.DeleteAsync(id);
    }
    public async Task<List<EnrollmentDto>> GetByUserIdAsync(Guid userId)
    {
        var enrollments = await _enrollementRepository.GetByUserIdAsync(userId);
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status
        }).ToList();
    }

    public async Task<EnrollmentDto> GetByUserAndCourseAsync(Guid userId, Guid courseId)
    {
        var enrollment = await _enrollementRepository.GetByUserAndCourseAsync(userId, courseId);
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
    public async Task<List<EnrollmentDto>> GetByCourseIdAsync(Guid courseId)
    {
        var enrollments = await _enrollementRepository.GetByCourseIdAsync(courseId);
        return enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            UserId = e.UserId,
            CourseId = e.CourseId,
            EnrolledAt = e.EnrolledAt,
            Status = e.Status
        }).ToList();
    }
}