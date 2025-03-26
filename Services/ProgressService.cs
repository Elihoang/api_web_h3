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
            LastUpdate = p.LastUpdate
        }).ToList();
    }

    public async Task<ProgressDto> GetByIdAsync(Guid id)
    {
        var progress = await _progressRepository.GetByIdAsync(id);
        if (progress == null)
        {
            return null;
        }
        return new ProgressDto
        {
            Id = progress.Id,
            UserId = progress.UserId,
            LessonId = progress.LessonId,
            Status = progress.Status,
            LastUpdate = progress.LastUpdate
        };
    }

    public async Task<ProgressDto> CreateAsync(CreateProgressDto createProgressDto)
    {
        if (await _progressRepository.ExistsByUserIdAndLessonIdAsync(createProgressDto.UserId, createProgressDto.LessonId))
        {
            throw new InvalidOperationException($"Trùng lặp userId {createProgressDto.UserId} Và lessonId {createProgressDto.LessonId} đã tồn tại.");
        }
        var progress = new Progress
        {
            Id = Guid.NewGuid(),
            UserId = createProgressDto.UserId,
            LessonId = createProgressDto.LessonId,
            Status = createProgressDto.Status,
            LastUpdate = DateTime.UtcNow
        };
        var createdProgress = await _progressRepository.CreateAsync(progress);
        return new ProgressDto
        {
            Id = createdProgress.Id,
            UserId = createdProgress.UserId,
            LessonId = createdProgress.LessonId,
            Status = createdProgress.Status,
            LastUpdate = createdProgress.LastUpdate
        };
    }

    public async Task<ProgressDto> UpdateAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        var progress = await _progressRepository.GetByIdAsync(id);
        if (progress == null)
        {
            return null;
        }
        progress.Status = updateProgressDto.Status;
        progress.LastUpdate = DateTime.UtcNow;
        
        var UpdatedProgress = await _progressRepository.UpdateAsync(progress);
        return new ProgressDto
        {
            Id = UpdatedProgress.Id,
            UserId = UpdatedProgress.UserId,
            LessonId = UpdatedProgress.LessonId,
            Status = UpdatedProgress.Status,
            LastUpdate = UpdatedProgress.LastUpdate

        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _progressRepository.DeleteAsync(id);
        
    }
}