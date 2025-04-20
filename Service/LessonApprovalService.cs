using API_WebH3.DTO.LessonApproval;
using API_WebH3.Models;
using API_WebH3.Repository;

namespace API_WebH3.Service;

public class LessonApprovalService
{
    private readonly ILessonApprovalRepository _lessonApprovalRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserRepository _userRepository;

    public LessonApprovalService(ILessonApprovalRepository lessonApprovalRepository,
        ILessonRepository lessonRepository, IUserRepository userRepository)
    {
        _lessonApprovalRepository = lessonApprovalRepository;
        _lessonRepository = lessonRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<LessonApprovalDto>> GetAllLessonApprovals()
    {
        var lessonApprovals = await _lessonApprovalRepository.GetAllAsync();
        return lessonApprovals.Select(la => new LessonApprovalDto
        {
            Id = la.Id,
            LessonId = la.LessonId,
            AdminId = la.AdminId,
            Action = la.Action,
            Comments = la.Comments,
            CreatedAt = la.CreatedAt
        });
    }

    public async Task<LessonApprovalDto> GetLessonApprovalById(Guid id)
    {
        var lessonApproval = await _lessonApprovalRepository.GetByIdAsync(id);
        if (lessonApproval == null)
        {
            return null;
        }

        return new LessonApprovalDto
        {
            Id = lessonApproval.Id,
            LessonId = lessonApproval.LessonId,
            AdminId = lessonApproval.AdminId,
            Action = lessonApproval.Action,
            Comments = lessonApproval.Comments,
            CreatedAt = lessonApproval.CreatedAt
        };
    }

    public async Task<LessonApprovalDto> CreateLessonApproval(CreateLessonApprovalDto createLessonApprovalDto)
    {
        var lesson = await _lessonRepository.GetLessonById(createLessonApprovalDto.LessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }
        var admin = await _userRepository.GetByIdAsync(createLessonApprovalDto.AdminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new ArgumentException("Admin not found or user is not an Admin.");
        }
        var lessonApproval = new LessonApproval
        {
            LessonId = createLessonApprovalDto.LessonId,
            AdminId = createLessonApprovalDto.AdminId,
            Action = createLessonApprovalDto.Action,
            Comments = createLessonApprovalDto.Comments,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };
        await _lessonApprovalRepository.AddAsync(lessonApproval);
        
        lesson.Status = createLessonApprovalDto.Action;
        lesson.ApprovedBy = createLessonApprovalDto.Action == "Approved" || createLessonApprovalDto.Action == "Rejected" ? createLessonApprovalDto.AdminId : null;
        await _lessonRepository.UpdateLesson(lesson);
        return new LessonApprovalDto
        {
            Id = lessonApproval.Id,
            LessonId = lessonApproval.LessonId,
            AdminId = lessonApproval.AdminId,
            Action = lessonApproval.Action,
            Comments = lessonApproval.Comments,
            CreatedAt = lessonApproval.CreatedAt
        };
    }

    public async Task<LessonApprovalDto> UpdateLessonApproval(Guid id, UpdateLessonApprovalDto updateLessonApprovalDto)
    {
        var lessonApproval = await _lessonApprovalRepository.GetByIdAsync(id);
        if (lessonApproval == null)
        {
            return null;
        }
        var lesson = await _lessonRepository.GetLessonById(updateLessonApprovalDto.LessonId);
        if (lesson == null)
        {
            throw new ArgumentException("Lesson not found.");
        }
        var admin = await _userRepository.GetByIdAsync(updateLessonApprovalDto.AdminId);
        if (admin == null || admin.Role != "Admin")
        {
            throw new ArgumentException("Admin not found or user is not an Admin.");
        }
        lessonApproval.LessonId = updateLessonApprovalDto.LessonId;
        lessonApproval.AdminId = updateLessonApprovalDto.AdminId;
        lessonApproval.Action = updateLessonApprovalDto.Action;
        lessonApproval.Comments = updateLessonApprovalDto.Comments;

        await _lessonApprovalRepository.UpdateAsync(lessonApproval);
        lesson.Status = updateLessonApprovalDto.Action;
        lesson.ApprovedBy = updateLessonApprovalDto.Action == "Approved" || updateLessonApprovalDto.Action == "Rejected" ? updateLessonApprovalDto.AdminId : null;
        await _lessonRepository.UpdateLesson(lesson);

        return new LessonApprovalDto
        {
            Id = lessonApproval.Id,
            LessonId = lessonApproval.LessonId,
            AdminId = lessonApproval.AdminId,
            Action = lessonApproval.Action,
            Comments = lessonApproval.Comments,
            CreatedAt = lessonApproval.CreatedAt
        };
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var lessonApproval = await _lessonApprovalRepository.GetByIdAsync(id);
        if (lessonApproval == null)
        {
            return false;
        }

        var lesson = await _lessonRepository.GetLessonById(lessonApproval.LessonId);
        if (lesson != null)
        {
            lesson.Status = "Pending";
            lesson.ApprovedBy = null;
            await _lessonRepository.UpdateLesson(lesson);
        }

        await _lessonApprovalRepository.DeleteAsync(id);
        return true;
    }
}