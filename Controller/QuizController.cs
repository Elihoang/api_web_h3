using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using API_WebH3.Models.DTO;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authorization;

namespace API_WebH3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController : ControllerBase
{
    private readonly QuizService _quizService;

    public QuizController(QuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuizDTO>>> GetAll()
    {
        var quizzes = await _quizService.GetAllAsync();
        return Ok(quizzes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuizDTO>> GetById(string id)
    {
        try
        {
            var quiz = await _quizService.GetByIdAsync(id);
            return Ok(quiz);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("user-answers/lesson/{lessonId}")]
    public async Task<ActionResult<IEnumerable<UserQuizAnswerDto>>> GetUserAnswersByLessonId(string lessonId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException("Người dùng chưa xác thực."));
            var userAnswers = await _quizService.GetUserAnswersByLessonIdAsync(lessonId, userId);
            return Ok(userAnswers ?? new List<UserQuizAnswerDto>());
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserAnswersByLessonId: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<IEnumerable<QuizDTO>>> GetByLessonId(string lessonId)
    {
        try
        {
            var quizzes = await _quizService.GetByLessonIdAsync(lessonId);
            return Ok(quizzes);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuizDTO>> Create([FromBody] CreateQuizDto quizDto)
    {
        try
        {
            Console.WriteLine($"Received Content-Type: {Request.ContentType}");
            Console.WriteLine($"Request Body: {System.Text.Json.JsonSerializer.Serialize(quizDto)}");

            var createdQuiz = await _quizService.CreateAsync(quizDto);
            return CreatedAtAction(nameof(GetById), new { id = createdQuiz.Id }, createdQuiz);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Create: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<QuizDTO>> Update(string id, [FromBody] QuizDTO quizDto)
    {
        try
        {
            var updatedQuiz = await _quizService.UpdateAsync(id, quizDto);
            return Ok(updatedQuiz);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var result = await _quizService.DeleteAsync(id);
        if (!result) return NotFound("Quiz không tìm thấy.");
        return NoContent();
    }

    [Authorize]
    [HttpPost("submit-answer")]
    public async Task<ActionResult<UserQuizAnswerDto>> SubmitAnswer([FromBody] QuizAnswerDto answerDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException("Người dùng chưa xác thực."));
            Console.WriteLine($"Received Submit Answer Request - QuizId: {answerDto.QuizId}, UserAnswer: {answerDto.UserAnswer}");

            var userAnswer = await _quizService.SaveUserAnswerAsync(answerDto.QuizId, userId, answerDto.UserAnswer);
            Console.WriteLine($"UserAnswer Result - IsCorrect: {userAnswer.IsCorrect}, Feedback: {userAnswer.Feedback}");

            return Ok(new UserQuizAnswerDto
            {
                Id = userAnswer.Id,
                QuizId = userAnswer.QuizId,
                UserAnswer = userAnswer.UserAnswer,
                IsCorrect = userAnswer.IsCorrect,
                Feedback = userAnswer.Feedback ?? "Không có phản hồi",
                AnsweredAt = userAnswer.AnsweredAt
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SubmitAnswer: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }

    [HttpDelete("user-answers/lesson/{lessonId}")]
    public async Task<ActionResult> DeleteUserAnswersByLessonId(string lessonId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new UnauthorizedAccessException("Người dùng chưa xác thực."));
            await _quizService.DeleteUserAnswersByLessonIdAsync(lessonId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteUserAnswersByLessonId: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }
}