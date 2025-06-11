using API_WebH3.Helpers;
using API_WebH3.Models;
using API_WebH3.Models.DTO;
using API_WebH3.Repositories;
using API_WebH3.Repository;

namespace API_WebH3.Services;

public class QuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserQuizAnswerRepository _userQuizAnswerRepository;

    public QuizService(IQuizRepository quizRepository, ILessonRepository lessonRepository, IUserQuizAnswerRepository userQuizAnswerRepository)
    {
        _userQuizAnswerRepository = userQuizAnswerRepository;
        _quizRepository = quizRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<IEnumerable<QuizDTO>> GetAllAsync()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        var quizDtos = new List<QuizDTO>();
        foreach (var quiz in quizzes)
        {
            quizDtos.Add(new QuizDTO
            {
                Id = quiz.Id,
                LessonId = quiz.LessonId,
                Question = quiz.Question,
                Options = System.Text.Json.JsonSerializer.Deserialize<List<string>>(quiz.SerializedOptions),
                CorrectAnswer = quiz.CorrectAnswer,
                Explanation = quiz.Explanation,
                CreatedAt = quiz.CreatedAt
            });
        }
        return quizDtos;
    }

    public async Task<QuizDTO> GetByIdAsync(string id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null)
        {
            AppLogger.LogError($"Quiz không tìm thấy với Id: {id}");
            throw new KeyNotFoundException("Quiz không tìm thấy.");
        }

        return new QuizDTO
        {
            Id = quiz.Id,
            LessonId = quiz.LessonId,
            Question = quiz.Question,
            Options = System.Text.Json.JsonSerializer.Deserialize<List<string>>(quiz.SerializedOptions),
            CorrectAnswer = quiz.CorrectAnswer,
            Explanation = quiz.Explanation,
            CreatedAt = quiz.CreatedAt
        };
    }

    public async Task<IEnumerable<QuizDTO>> GetByLessonIdAsync(string lessonId)
    {
        var lesson = await _lessonRepository.GetLessonById(lessonId);
        if (lesson == null) 
        {
            AppLogger.LogError($"Bài học không tìm thấy với Id: {lessonId}");
            throw new KeyNotFoundException("Bài học không tìm thấy.");
        }

        var quizzes = await _quizRepository.GetByLessonIdAsync(lessonId);
        var quizDtos = new List<QuizDTO>();
        foreach (var quiz in quizzes)
        {
            quizDtos.Add(new QuizDTO
            {
                Id = quiz.Id,
                LessonId = quiz.LessonId,
                Question = quiz.Question,
                Options = System.Text.Json.JsonSerializer.Deserialize<List<string>>(quiz.SerializedOptions),
                CorrectAnswer = quiz.CorrectAnswer,
                Explanation = quiz.Explanation,
                CreatedAt = quiz.CreatedAt
            });
        }
        return quizDtos;
    }

    public async Task<QuizDTO> CreateAsync(CreateQuizDto createQuizDto)
{
    try
    {
        if (string.IsNullOrWhiteSpace(createQuizDto.Question))
        {
            AppLogger.LogInfo("Câu hỏi không được để trống.");
            throw new ArgumentException("Câu hỏi không được để trống.");
        }

        if (createQuizDto.Options == null || createQuizDto.Options.Count < 2)
        {
            AppLogger.LogInfo("Quiz phải có ít nhất 2 lựa chọn.");
            throw new ArgumentException("Quiz phải có ít nhất 2 lựa chọn.");
        }

        if (string.IsNullOrWhiteSpace(createQuizDto.CorrectAnswer))
        {
            AppLogger.LogInfo("Đáp án đúng không được để trống.");
            throw new ArgumentException("Đáp án đúng không được để trống.");
        }

        if (!createQuizDto.Options.Contains(createQuizDto.CorrectAnswer))
        {
            AppLogger.LogInfo("Đáp án đúng phải nằm trong danh sách lựa chọn.");
            throw new ArgumentException("Đáp án đúng phải nằm trong danh sách lựa chọn.");
        }

        AppLogger.LogInfo($"Checking LessonId: {createQuizDto.LessonId}");
        var lesson = await _lessonRepository.GetLessonById(createQuizDto.LessonId);
        if (lesson == null) throw new KeyNotFoundException("Bài học không tìm thấy.");

        var quiz = new Quiz
        {
            Id = Helpers.IdGenerator.IdQuiz(),
            LessonId = createQuizDto.LessonId,
            Question = createQuizDto.Question,
            Options = createQuizDto.Options,
            CorrectAnswer = createQuizDto.CorrectAnswer,
            Explanation = createQuizDto.Explanation,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") // Server tự tạo
        };

        AppLogger.LogInfo($"Saving Quiz with Id: {quiz.Id}");
        var createdQuiz = await _quizRepository.AddAsync(quiz);

        return new QuizDTO
        {
            Id = createdQuiz.Id,
            LessonId = createdQuiz.LessonId,
            Question = createdQuiz.Question,
            Options = createdQuiz.Options,
            CorrectAnswer = createdQuiz.CorrectAnswer,
            Explanation = createdQuiz.Explanation,
            CreatedAt = createdQuiz.CreatedAt
        };
    }
    catch (Exception ex)
    {
        AppLogger.LogError($"Error in CreateAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
        throw;
    }
}

    public async Task<QuizDTO> UpdateAsync(string id, QuizDTO quizDto)
    {
        var existingQuiz = await _quizRepository.GetByIdAsync(id);
        if (existingQuiz == null)
        {
            AppLogger.LogError($"Quiz không tìm thấy với Id: {id}");
            throw new KeyNotFoundException("Quiz không tìm thấy.");
        }

        if (string.IsNullOrWhiteSpace(quizDto.Question))
        {
            AppLogger.LogError("Câu hỏi không được để trống.");
            throw new ArgumentException("Câu hỏi không được để trống.");
        }

        if (quizDto.Options == null || quizDto.Options.Count < 2)
        {
            AppLogger.LogError("Quiz phải có ít nhất 2 lựa chọn.");
            throw new ArgumentException("Quiz phải có ít nhất 2 lựa chọn.");
        }

        if (string.IsNullOrWhiteSpace(quizDto.CorrectAnswer))
        {
            AppLogger.LogError("Đáp án đúng không được để trống.");
            throw new ArgumentException("Đáp án đúng không được để trống.");
        }

        if (!quizDto.Options.Contains(quizDto.CorrectAnswer))
        {
            AppLogger.LogError("Đáp án đúng phải nằm trong danh sách lựa chọn.");
            throw new ArgumentException("Đáp án đúng phải nằm trong danh sách lựa chọn.");
        }

        var lesson = await _lessonRepository.GetLessonById(quizDto.LessonId);
        if (lesson == null) throw new KeyNotFoundException("Bài học không tìm thấy.");

        existingQuiz.LessonId = quizDto.LessonId;
        existingQuiz.Question = quizDto.Question;
        existingQuiz.Options = quizDto.Options;
        existingQuiz.CorrectAnswer = quizDto.CorrectAnswer;
        existingQuiz.Explanation = quizDto.Explanation;
        existingQuiz.CreatedAt = existingQuiz.CreatedAt;

        var updatedQuiz = await _quizRepository.UpdateAsync(existingQuiz);

        return new QuizDTO
        {
            Id = updatedQuiz.Id,
            LessonId = updatedQuiz.LessonId,
            Question = updatedQuiz.Question,
            Options = updatedQuiz.Options,
            CorrectAnswer = updatedQuiz.CorrectAnswer,
            Explanation = updatedQuiz.Explanation,
            CreatedAt = updatedQuiz.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _quizRepository.DeleteAsync(id);
    }

    public async Task<bool> CheckAnswerAsync(string quizId, string userAnswer)
    {
        try
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                AppLogger.LogError($"Quiz không tìm thấy với Id: {quizId}");
                throw new KeyNotFoundException("Quiz không tìm thấy.");
            }

            return quiz.CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in CheckAnswerAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<UserQuizAnswer> SaveUserAnswerAsync(string quizId, Guid userId, string userAnswer)
    {
        try
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                AppLogger.LogError($"Quiz không tìm thấy với Id: {quizId}");
                throw new KeyNotFoundException("Quiz không tìm thấy.");
            }

            var isCorrect = quiz.CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);

            var existingAnswer = (await _userQuizAnswerRepository.GetByQuizIdAsync(quizId))
                .FirstOrDefault(a => a.UserId == userId);

            var userAnswerRecord = new UserQuizAnswer
            {
                UserId = userId,
                QuizId = quizId,
                UserAnswer = userAnswer,
                IsCorrect = isCorrect,
                Feedback = isCorrect ? null : $"Đáp án đúng là: {quiz.CorrectAnswer}",
                AnsweredAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
            };

            if (existingAnswer != null)
            {
                existingAnswer.UserAnswer = userAnswer;
                existingAnswer.IsCorrect = isCorrect;
                existingAnswer.Feedback = userAnswerRecord.Feedback;
                existingAnswer.AnsweredAt = userAnswerRecord.AnsweredAt;

                await _userQuizAnswerRepository.UpdateAsync(existingAnswer);
                return existingAnswer;
            }
            else
            {
                var createdAnswer = await _userQuizAnswerRepository.AddAsync(userAnswerRecord);
                return createdAnswer;
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in SaveUserAnswerAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task DeleteUserAnswersByLessonIdAsync(string lessonId, Guid userId)
    {
        try
        {
            var userAnswers = await _userQuizAnswerRepository.GetByLessonIdAsync(lessonId, userId);
            foreach (var answer in userAnswers)
            {
                await _userQuizAnswerRepository.DeleteAsync(answer.Id);
            }
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in DeleteUserAnswersByLessonIdAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<IEnumerable<UserQuizAnswerDto>> GetUserAnswersByLessonIdAsync(string lessonId, Guid userId)
    {
        try
        {
            var userAnswers = await _userQuizAnswerRepository.GetByLessonIdAsync(lessonId, userId);
            var answerDtos = userAnswers.Select(answer => new UserQuizAnswerDto
            {
                Id = answer.Id,
                QuizId = answer.QuizId,
                UserAnswer = answer.UserAnswer,
                IsCorrect = answer.IsCorrect,
                Feedback = answer.Feedback ?? "Không có phản hồi",
                AnsweredAt = answer.AnsweredAt
            }).ToList();
            return answerDtos;
        }
        catch (Exception ex)
        {
            AppLogger.LogError($"Error in GetUserAnswersByLessonIdAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }
}