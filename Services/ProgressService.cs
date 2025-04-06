using API_WebH3.DTOs.Progress;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class ProgressService
{
    private readonly IProgressRepository _progressRepository;

    public ProgressService(IProgressRepository progressRepository)
    {
        _progressRepository = progressRepository;
    }

    public async Task<List<ProgressDto>> GetAllAsync()
    {
        var progresses = await _progressRepository.GetAllAsync();
        return progresses.Select(p => new ProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            LessonId = p.LessonId,
            Status = p.Status,
            CompletionPercentage = p.CompletionPercentage,
            Notes = p.Notes,
            LastUpdate = p.LastUpdate
        }).ToList();
    }

    public async Task<ProgressDto?> GetByIdAsync(Guid id)
    {
        var progress = await _progressRepository.GetByIdAsync(id);
        if (progress == null) return null;
        return new ProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            LessonId = progress.LessonId,
            Status = progress.Status,
            CompletionPercentage = progress.CompletionPercentage,
            Notes = progress.Notes,
            LastUpdate = progress.LastUpdate
        };
    }

    public async Task<ProgressDto> CreateAsync(CreateProgressDto createProgressDto)
    {
        var progress = new Progress
        {
            UserId = createProgressDto.UserId,
            LessonId = createProgressDto.LessonId,
            Status = createProgressDto.Status,
            CompletionPercentage = createProgressDto.CompletionPercentage,
            Notes = createProgressDto.Notes,
            LastUpdate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        var createdProgress = await _progressRepository.CreateAsync(progress);
        return new ProgressDto
        {
            Id = createdProgress.Id,
            UserId = createdProgress.UserId,
            LessonId = createdProgress.LessonId,
            Status = createdProgress.Status,
            CompletionPercentage = createdProgress.CompletionPercentage,
            Notes = createdProgress.Notes,
            LastUpdate = createdProgress.LastUpdate
        };
    }

    public async Task<ProgressDto?> UpdateAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        var existingProgress = await _progressRepository.GetByIdAsync(id);
        if (existingProgress == null) return null;

        if (updateProgressDto.Status != null) existingProgress.Status = updateProgressDto.Status;
        if (updateProgressDto.CompletionPercentage.HasValue) existingProgress.CompletionPercentage = updateProgressDto.CompletionPercentage.Value;
        if (updateProgressDto.Notes != null) existingProgress.Notes = updateProgressDto.Notes;
        existingProgress.LastUpdate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        var updatedProgress = await _progressRepository.UpdateAsync(existingProgress);
        if (updatedProgress == null) return null;
        return new ProgressDto
        {
            Id = updatedProgress.Id,
            UserId = updatedProgress.UserId,
            LessonId = updatedProgress.LessonId,
            Status = updatedProgress.Status,
            CompletionPercentage = updatedProgress.CompletionPercentage,
            Notes = updatedProgress.Notes,
            LastUpdate = updatedProgress.LastUpdate
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _progressRepository.DeleteAsync(id);
    }

    public async Task<List<ProgressDto>> GetByUserIdAsync(Guid userId)
    {
        var progresses = await _progressRepository.GetByUserIdAsync(userId);
        return progresses.Select(p => new ProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            LessonId = p.LessonId,
            Status = p.Status,
            CompletionPercentage = p.CompletionPercentage,
            Notes = p.Notes,
            LastUpdate = p.LastUpdate
        }).ToList();
    }

    public async Task<List<ProgressDto>> GetByLessonIdAsync(Guid lessonId)
    {
        var progresses = await _progressRepository.GetByLessonIdAsync(lessonId);
        return progresses.Select(p => new ProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            LessonId = p.LessonId,
            Status = p.Status,
            CompletionPercentage = p.CompletionPercentage,
            Notes = p.Notes,
            LastUpdate = p.LastUpdate
        }).ToList();
    }

    public async Task<ProgressDto?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
    {
        var progress = await _progressRepository.GetByUserAndLessonAsync(userId, lessonId);
        if (progress == null) return null;
        return new ProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            LessonId = progress.LessonId,
            Status = progress.Status,
            CompletionPercentage = progress.CompletionPercentage,
            Notes = progress.Notes,
            LastUpdate = progress.LastUpdate
        };
    }

    public async Task<List<ProgressDto>> GetByCourseIdAsync(Guid courseId, Guid userId)
    {
        var progresses = await _progressRepository.GetByCourseIdAsync(courseId, userId);
        return progresses.Select(p => new ProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            LessonId = p.LessonId,
            Status = p.Status,
            CompletionPercentage = p.CompletionPercentage,
            Notes = p.Notes,
            LastUpdate = p.LastUpdate
        }).ToList();
    }
}