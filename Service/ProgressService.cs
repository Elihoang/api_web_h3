using API_WebH3.DTO.Progress;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class ProgressService
{
    private readonly IProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILessonRepository _lessonRepository;
    
    public ProgressService(IProgressRepository progressRepository, IUserRepository userRepository, ILessonRepository lessonRepository)
    {
        _progressRepository = progressRepository;
        _userRepository = userRepository;
        _lessonRepository = lessonRepository;
    }
    public async Task<IEnumerable<ProgressDto>> GetAllAsync()
    {
        var progresses = await _progressRepository.GetAllProgressAsync();
        return progresses.Select(p => new ProgressDto
        {
            Id = p.Id,
            UserId = p.UserId,
            LessonId = p.LessonId,
            Status = p.Status,
            CompletionPercentage = p.CompletionPercentage,
            Notes = p.Notes,
            LastUpdate = p.LastUpdate
        });
    }

    public async Task<ProgressDto> GetByIdAsync(Guid id)
    {
        var progress = await _progressRepository.GetProgressByIdAsync(id);
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
            CompletionPercentage = progress.CompletionPercentage,
            Notes = progress.Notes,
            LastUpdate = progress.LastUpdate
        };
    }

    public async Task<ProgressDto> CreateAsync(CreateProgressDto createProgressDto)
    {
        var user = await _userRepository.GetByIdAsync(createProgressDto.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        var lesson = await _lessonRepository.GetLessonById(createProgressDto.LessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }

        var existingProgress = await _progressRepository.GetByUserAndLessonAsync(createProgressDto.UserId, createProgressDto.LessonId);
        if (existingProgress != null)
        {
            throw new ArgumentException("Progress for this user and lesson already exists.");
        }

        var validStatuses = new[] { "not started", "in progress", "completed" };
        if (!validStatuses.Contains(createProgressDto.Status.ToLower()))
        {
            throw new ArgumentException("Invalid status. Must be 'not started', 'in progress', or 'completed'.");
        }

        if (createProgressDto.CompletionPercentage < 0 || createProgressDto.CompletionPercentage > 100)
        {
            throw new ArgumentException("Completion percentage must be between 0 and 100.");
        }

        var progress = new Progress
        {
            Id = Guid.NewGuid(),
            UserId = createProgressDto.UserId,
            LessonId = createProgressDto.LessonId,
            Status = createProgressDto.Status.ToLower(),
            CompletionPercentage = createProgressDto.CompletionPercentage,
            Notes = createProgressDto.Notes,
            LastUpdate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        await _progressRepository.AddProgressAsync(progress);

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

    public async Task<ProgressDto> UpdateAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        var progress = await _progressRepository.GetProgressByIdAsync(id);
        if (progress == null)
        {
            return null;
        }

        var validStatuses = new[] { "not started", "in progress", "completed" };
        if (!validStatuses.Contains(updateProgressDto.Status.ToLower()))
        {
            throw new ArgumentException("Invalid status. Must be 'not started', 'in progress', or 'completed'.");
        }
        
        if (updateProgressDto.CompletionPercentage < 0 || updateProgressDto.CompletionPercentage > 100)
        {
            throw new ArgumentException("Completion percentage must be between 0 and 100.");
        }

        progress.Status = updateProgressDto.Status.ToLower();
        progress.CompletionPercentage = updateProgressDto.CompletionPercentage;
        progress.Notes = updateProgressDto.Notes;
        progress.LastUpdate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

        await _progressRepository.UpdateProgressAsync(progress);

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

    public async Task<bool> DeleteAsync(Guid id)
    {
        var progress = await _progressRepository.GetProgressByIdAsync(id);
        if (progress == null)
        {
            return false;
        }

        await _progressRepository.DeleteProgressAsync(id);
        return true;
    }
}