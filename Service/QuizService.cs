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
                Options = quiz.Options,
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
        if (quiz == null) throw new KeyNotFoundException("Quiz không tìm thấy.");

        return new QuizDTO
        {
            Id = quiz.Id,
            LessonId = quiz.LessonId,
            Question = quiz.Question,
            Options = quiz.Options,
            CorrectAnswer = quiz.CorrectAnswer,
            Explanation = quiz.Explanation,
            CreatedAt = quiz.CreatedAt
        };
    }

    public async Task<IEnumerable<QuizDTO>> GetByLessonIdAsync(string lessonId)
    {
        var lesson = await _lessonRepository.GetLessonById(lessonId);
        if (lesson == null) throw new KeyNotFoundException("Bài học không tìm thấy.");

        var quizzes = await _quizRepository.GetByLessonIdAsync(lessonId);
        var quizDtos = new List<QuizDTO>();
        foreach (var quiz in quizzes)
        {
            quizDtos.Add(new QuizDTO
            {
                Id = quiz.Id,
                LessonId = quiz.LessonId,
                Question = quiz.Question,
                Options = quiz.Options,
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
        // Kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(createQuizDto.Question))
            throw new ArgumentException("Câu hỏi không được để trống.");

        if (createQuizDto.Options == null || createQuizDto.Options.Count < 2)
            throw new ArgumentException("Quiz phải có ít nhất 2 lựa chọn.");

        if (string.IsNullOrWhiteSpace(createQuizDto.CorrectAnswer))
            throw new ArgumentException("Đáp án đúng không được để trống.");

        if (!createQuizDto.Options.Contains(createQuizDto.CorrectAnswer))
            throw new ArgumentException("Đáp án đúng phải nằm trong danh sách lựa chọn.");

        Console.WriteLine($"Checking LessonId: {createQuizDto.LessonId}");
        var lesson = await _lessonRepository.GetLessonById(createQuizDto.LessonId);
        if (lesson == null) throw new KeyNotFoundException("Bài học không tìm thấy.");

        // Ánh xạ thủ công sang entity
        var quiz = new Quiz
        {
            Id = Helpers.IdGenerator.IdQuiz(),
            LessonId = createQuizDto.LessonId,
            Question = createQuizDto.Question,
            Options = createQuizDto.Options,
            CorrectAnswer = createQuizDto.CorrectAnswer,
            Explanation = createQuizDto.Explanation,
            CreatedAt = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
        };

        Console.WriteLine($"Saving Quiz with Id: {quiz.Id}");
        // Lưu vào cơ sở dữ liệu
        var createdQuiz = await _quizRepository.AddAsync(quiz);

        // Ánh xạ thủ công sang DTO
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
        // Log lỗi chi tiết
        Console.WriteLine($"Error in CreateAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
        throw;
    }
}

    public async Task<QuizDTO> UpdateAsync(string id, QuizDTO quizDto)
    {
        // Tìm quiz hiện có
        var existingQuiz = await _quizRepository.GetByIdAsync(id);
        if (existingQuiz == null) throw new KeyNotFoundException("Quiz không tìm thấy.");

        // Kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(quizDto.Question))
            throw new ArgumentException("Câu hỏi không được để trống.");

        if (quizDto.Options == null || quizDto.Options.Count < 2)
            throw new ArgumentException("Quiz phải có ít nhất 2 lựa chọn.");

        if (string.IsNullOrWhiteSpace(quizDto.CorrectAnswer))
            throw new ArgumentException("Đáp án đúng không được để trống.");

        if (!quizDto.Options.Contains(quizDto.CorrectAnswer))
            throw new ArgumentException("Đáp án đúng phải nằm trong danh sách lựa chọn.");

        var lesson = await _lessonRepository.GetLessonById(quizDto.LessonId);
        if (lesson == null) throw new KeyNotFoundException("Bài học không tìm thấy.");

        // Ánh xạ thủ công sang entity
        existingQuiz.LessonId = quizDto.LessonId;
        existingQuiz.Question = quizDto.Question;
        existingQuiz.Options = quizDto.Options;
        existingQuiz.CorrectAnswer = quizDto.CorrectAnswer;
        existingQuiz.Explanation = quizDto.Explanation;
        existingQuiz.CreatedAt = existingQuiz.CreatedAt; // Giữ nguyên CreatedAt

        // Cập nhật cơ sở dữ liệu
        var updatedQuiz = await _quizRepository.UpdateAsync(existingQuiz);

        // Ánh xạ thủ công sang DTO
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
                throw new KeyNotFoundException("Quiz không tìm thấy.");

            // So sánh đáp án người dùng với đáp án đúng (không phân biệt chữ hoa/thường)
            return quiz.CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckAnswerAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }
    public async Task<UserQuizAnswer> SaveUserAnswerAsync(string quizId, Guid userId, string userAnswer)
    {
        try
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
                throw new KeyNotFoundException("Quiz không tìm thấy.");

            var isCorrect = quiz.CorrectAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);

            var userAnswerRecord = new UserQuizAnswer
            {
                UserId = userId,
                QuizId = quizId,
                UserAnswer = userAnswer,
                IsCorrect = isCorrect,
                Feedback = isCorrect ? null : $"Đáp án đúng là: {quiz.CorrectAnswer}"
            };

            // Giả sử có repository cho UserQuizAnswer (cần tạo IUserQuizAnswerRepository)
            var createdAnswer = await _userQuizAnswerRepository.AddAsync(userAnswerRecord);
            return createdAnswer;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveUserAnswerAsync: {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }
    
}