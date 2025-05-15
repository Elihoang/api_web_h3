using API_WebH3.DTO.Lesson;
using API_WebH3.Helpers;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class LessonService
{
    private readonly ILessonRepository _repository;

    public LessonService(ILessonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LessonDto>> GetAllLessons()
    {
        var lessons = await _repository.GetAllLessons();
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            ChapterId = l.ChapterId,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            VideoName = l.VideoName,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            Status = l.Status,
            ApprovedBy = l.ApprovedBy,
            CreatedAt = l.CreatedAt
        });
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByChapterId(string chapterId)
    {
        var lessons = await _repository.GetLessonsByChapterIdAsync(chapterId);
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            ChapterId = l.ChapterId,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            VideoName = l.VideoName,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            Status = l.Status,
            ApprovedBy = l.ApprovedBy,
            CreatedAt = l.CreatedAt
        });
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseId(string courseId)
    {
        var lessons = await _repository.GetLessonsByCourseIdAsync(courseId);
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            ChapterId = l.ChapterId,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            VideoName = l.VideoName,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            Status = l.Status,
            ApprovedBy = l.ApprovedBy,
            CreatedAt = l.CreatedAt
        });
    }

    public async Task<LessonDto> GetLessonById(string id)
    {
        var lesson = await _repository.GetLessonById(id);
        if (lesson == null)
        {
            return null;
        }

        return new LessonDto
        {
            Id = lesson.Id,
            ChapterId = lesson.ChapterId,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content,
            VideoName = lesson.VideoName,
            Duration = lesson.Duration,
            OrderNumber = lesson.OrderNumber,
            Status = lesson.Status,
            ApprovedBy = lesson.ApprovedBy,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<LessonDto> CreateLesson(CreateLessonDto createLessonDto)
    {
        var lesson = new Lesson
        {
            Id = IdGenerator.IdLesson(),
            ChapterId = createLessonDto.ChapterId,
            CourseId = createLessonDto.CourseId,
            Title = createLessonDto.Title,
            Description = createLessonDto.Description,
            Content = createLessonDto.Content,
            VideoName = createLessonDto.VideoName,
            Duration = createLessonDto.Duration,
            OrderNumber = createLessonDto.OrderNumber,
            Status = "Pending",
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        
        await _repository.CreateLesson(lesson);

        return new LessonDto
        {
            Id = lesson.Id,
            ChapterId = lesson.ChapterId,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content,
            VideoName = lesson.VideoName,
            Duration = lesson.Duration,
            OrderNumber = lesson.OrderNumber,
            Status = lesson.Status,
            ApprovedBy = lesson.ApprovedBy,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<LessonDto> UpdateLesson(string id, UpdateLessonDto updateLessonDto)
    {
        var lesson = await _repository.GetLessonById(id);
        if (lesson == null)
        {
            return null;
        }
        
        lesson.ChapterId = updateLessonDto.ChapterId;
        lesson.CourseId = updateLessonDto.CourseId;
        lesson.Title = updateLessonDto.Title;
        lesson.Description = updateLessonDto.Description;
        lesson.Content = updateLessonDto.Content;
        lesson.VideoName = updateLessonDto.VideoName;
        lesson.Duration = updateLessonDto.Duration;
        lesson.OrderNumber = updateLessonDto.OrderNumber;
        
        await _repository.UpdateLesson(lesson);

        return new LessonDto
        {
            Id = lesson.Id,
            ChapterId = lesson.ChapterId,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content,
            VideoName = lesson.VideoName,
            Duration = lesson.Duration,
            OrderNumber = lesson.OrderNumber,
            Status = lesson.Status,
            ApprovedBy = lesson.ApprovedBy,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<bool> DeleteLesson(string id)
    {
        var lesson = await _repository.GetLessonById(id);
        if (lesson == null)
        {
            return false;
        }
        await _repository.DeleteLesson(id);
        return true;
    }
    
    
}