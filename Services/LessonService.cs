using API_WebH3.DTOs.Lesson;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class LessonService
{
    private readonly ILessonRepository _repository;

    public LessonService(ILessonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LessonDto>> GetAllLessonAsync()
    {
        var lessons = await _repository.GetAllAsync();
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            CreatedAt = l.CreatedAt
        });
    }

    public async Task<LessonDto> GetLessonByIdAsync(Guid id)
    {
        var lesson = await _repository.GetByIdAsync(id);
        if (lesson == null)
        {
            return null;
        }

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var lessons = await _repository.GetByCourseIdAsync(courseId);
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            CreatedAt = l.CreatedAt
        });
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto)
    {
        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = createLessonDto.CourseId,
            Title = createLessonDto.Title,
            Content = createLessonDto.Content,
            VideoUrl = createLessonDto.VideoUrl,
            CreatedAt = DateTime.UtcNow
        };
        
        var createdLesson = await _repository.CreateAsync(lesson);
        return new LessonDto
        {
            Id = createdLesson.Id,
            CourseId = createdLesson.CourseId,
            Title = createdLesson.Title,
            Content = createdLesson.Content,
            VideoUrl = createdLesson.VideoUrl,
            CreatedAt = createdLesson.CreatedAt
        };
    }

    public async Task<LessonDto> UpdateLessonAsync(Guid id ,UpdateLessonDto updateLessonDto)
    {
        var existingLesson = await _repository.GetByIdAsync(id);
        if (existingLesson == null)
        {
            return null;
        }
        existingLesson.Title = updateLessonDto.Title;
        existingLesson.Content = updateLessonDto.Content;
        existingLesson.VideoUrl = updateLessonDto.VideoUrl;
        
        var updateLesson = await _repository.UpdateAsync(existingLesson);
        return new LessonDto
        {
            Id = updateLesson.Id,
            CourseId = updateLesson.CourseId,
            Title = updateLesson.Title,
            Content = updateLesson.Content,
            VideoUrl = updateLesson.VideoUrl,
            CreatedAt = updateLesson.CreatedAt
        };
    }

    public async Task<bool> DeleteLessonAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }
}