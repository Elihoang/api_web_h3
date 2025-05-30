using API_WebH3.DTO.Chapter;
using API_WebH3.Helpers;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class ChapterService
{
    private readonly IChapterRepository _chapterRepository;

    public ChapterService(IChapterRepository chapterRepository)
    {
        _chapterRepository = chapterRepository;
    }

    public async Task<IEnumerable<ChapterDto>> GetAllChapters()
    {
        var chapters = await _chapterRepository.GetAllChaptersAsync();
        return chapters.Select(c => new ChapterDto
        {
            Id = c.Id,
            CourseId = c.CourseId,
            Title = c.Title,
            Description = c.Description,
            OrderNumber = c.OrderNumber,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<IEnumerable<ChapterDto>> GetChaptersByCourseId(string courseId)
    {
        var chapters = await _chapterRepository.GetChaptersByCourseIdAsync(courseId);
        return chapters.Select(c => new ChapterDto
        {
            Id = c.Id,
            CourseId = c.CourseId,
            Title = c.Title,
            Description = c.Description,
            OrderNumber = c.OrderNumber,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<ChapterDto> GetChapterById(string id)
    {
        var chapter = await _chapterRepository.GetChapterByIdAsync(id);
        if (chapter == null)
        {
            return null;
        }

        return new ChapterDto
        {
            Id = chapter.Id,
            CourseId = chapter.CourseId,
            Title = chapter.Title,
            Description = chapter.Description,
            OrderNumber = chapter.OrderNumber,
            CreatedAt = chapter.CreatedAt
        };
    }

    public async Task<ChapterDto> CreateChapter(CreateChapterDto createChapterDto)
    {
        var chapter = new Chapter
        {
            Id = IdGenerator.IdChapter(),
            CourseId = createChapterDto.CourseId,
            Title = createChapterDto.Title,
            Description = createChapterDto.Description,
            OrderNumber = createChapterDto.OrderNumber,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        await _chapterRepository.AddChapterAsync(chapter);

        return new ChapterDto
        {
            Id = chapter.Id,
            CourseId = chapter.CourseId,
            Title = chapter.Title,
            Description = chapter.Description,
            OrderNumber = chapter.OrderNumber,
            CreatedAt = chapter.CreatedAt
        };
    }

    public async Task<ChapterDto> UpdateChapter(string id, UpdateChapterDto updateChapterDto)
    {
        var chapter = await _chapterRepository.GetChapterByIdAsync(id);
        if (chapter == null)
        {
            return null;
        }        
        chapter.Title = updateChapterDto.Title;
        chapter.Description = updateChapterDto.Description;
        chapter.OrderNumber = updateChapterDto.OrderNumber;
        await _chapterRepository.UpdateChapterAsync(chapter);

        return new ChapterDto
        {
            Id = chapter.Id,
            CourseId = chapter.CourseId,
            Title = chapter.Title,
            Description = chapter.Description,
            OrderNumber = chapter.OrderNumber,
            CreatedAt = chapter.CreatedAt
        };
    }

    public async Task<bool> DeleteChapter(string id)
    {
        var chapter = await _chapterRepository.GetChapterByIdAsync(id);
        if (chapter == null)
        {
            return false;
        }
        await _chapterRepository.DeleteChapterAsync(id);
        return true;
    }
}