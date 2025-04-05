using API_WebH3.DTOs.Lesson;
using API_WebH3.Models;
using API_WebH3.Repositories;

namespace API_WebH3.Services;

public class LessonService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<List<LessonDto>> GetAllAsync()
    {
        var lessons = await _lessonRepository.GetAllAsync();
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<LessonDto?> GetByIdAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
        {
            return null;
        }

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            Duration = lesson.Duration,
            OrderNumber = lesson.OrderNumber,
            CreatedAt = lesson.CreatedAt
        };
    }

    public async Task<LessonDto> CreateAsync(CreateLessonDto createLessonDto)
    {
        var lesson = new Lesson
        {
            CourseId = createLessonDto.CourseId,
            Title = createLessonDto.Title,
            Description = createLessonDto.Description,
            Content = createLessonDto.Content,
            VideoUrl = createLessonDto.VideoUrl,
            Duration = createLessonDto.Duration,
            OrderNumber = createLessonDto.OrderNumber
        };

        var createdLesson = await _lessonRepository.CreateAsync(lesson);
        return new LessonDto
        {
            Id = createdLesson.Id,
            CourseId = createdLesson.CourseId,
            Title = createdLesson.Title,
            Description = createdLesson.Description,
            Content = createdLesson.Content,
            VideoUrl = createdLesson.VideoUrl,
            Duration = createdLesson.Duration,
            OrderNumber = createdLesson.OrderNumber,
            CreatedAt = createdLesson.CreatedAt
        };
    }

    public async Task<LessonDto?> UpdateAsync(Guid id, UpdateLessonDto updateLessonDto)
    {
        var existingLesson = await _lessonRepository.GetByIdAsync(id);
        if (existingLesson == null)
        {
            return null;
        }

        if (updateLessonDto.Title != null)
            existingLesson.Title = updateLessonDto.Title;
        if (updateLessonDto.Description != null)
            existingLesson.Description = updateLessonDto.Description;
        if (updateLessonDto.Content != null)
            existingLesson.Content = updateLessonDto.Content;
        if (updateLessonDto.VideoUrl != null)
            existingLesson.VideoUrl = updateLessonDto.VideoUrl;
        if (updateLessonDto.Duration.HasValue)
            existingLesson.Duration = updateLessonDto.Duration.Value;
        if (updateLessonDto.OrderNumber.HasValue)
            existingLesson.OrderNumber = updateLessonDto.OrderNumber.Value;

        var updatedLesson = await _lessonRepository.UpdateAsync(existingLesson);
        if (updatedLesson == null)
        {
            return null;
        }

        return new LessonDto
        {
            Id = updatedLesson.Id,
            CourseId = updatedLesson.CourseId,
            Title = updatedLesson.Title,
            Description = updatedLesson.Description,
            Content = updatedLesson.Content,
            VideoUrl = updatedLesson.VideoUrl,
            Duration = updatedLesson.Duration,
            OrderNumber = updatedLesson.OrderNumber,
            CreatedAt = updatedLesson.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _lessonRepository.DeleteAsync(id);
    }

    public async Task<List<LessonDto>> GetByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            Duration = l.Duration,
            OrderNumber = l.OrderNumber,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<LessonDto?> GetByCourseIdAndOrderAsync(Guid courseId, int orderNumber)
    {
        var lesson = await _lessonRepository.GetByCourseIdAndOrderAsync(courseId, orderNumber);
        if (lesson == null)
        {
            return null;
        }

        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            Duration = lesson.Duration,
            OrderNumber = lesson.OrderNumber,
            CreatedAt = lesson.CreatedAt
        };
    }
}