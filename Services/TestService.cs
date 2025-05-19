using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO.Answer;
using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class TestService : ITestService
{
    private readonly IGenericRepository<Score> _scoreRepository;
    private readonly IGenericRepository<Question> _questionRepository;
    private readonly IGenericRepository<Response> _responseRepository;
    private readonly IGenericRepository<ResponseOption> _responseOptionRepository;
    private readonly CurseDbContext _context;
    private readonly ILogger<TestService> _logger;

    public TestService(IGenericRepository<Score> scoreRepository,
        IGenericRepository<Question> questionRepository,
        IGenericRepository<Response> responseRepository,
        IGenericRepository<ResponseOption> responseOptionRepository,
        ILogger<TestService> logger, CurseDbContext context)
    {
        _scoreRepository = scoreRepository;
        _questionRepository = questionRepository;
        _responseRepository = responseRepository;
        _responseOptionRepository = responseOptionRepository;
        _logger = logger;
        _context = context;
    }

    public async Task<TestResultDto> SubmitTestAnswersAsync(int userId, SubmitTestDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var score = new Score
            {
                UserId = userId,
                LearnMaterialId = dto.LearnMaterialId,
                Score1 = 0,
                CreatedAt = DateTime.Now
            };

            await _scoreRepository.CreateAsync(score);

            var materialQuestions = await _questionRepository.GetQueryable()
                .Where(q => q.LearnMaterialId == dto.LearnMaterialId)
                .Include(q => q.CorrectAnswers)
                .ToListAsync();

            foreach (var answer in dto.Answers)
            {
                var question = materialQuestions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                {
                    _logger.LogWarning($"Question {answer.QuestionId} not found in material {dto.LearnMaterialId}");
                    continue;
                }

                // Заменяем старую логику на вызов ProcessResponse
                await ProcessResponse(question, answer, score);
            }

            await _scoreRepository.UpdateAsync(score);
            await transaction.CommitAsync();

            return new TestResultDto
            {
                ScoreId = score.Id,
                TotalScore = score.Score1,
                CorrectAnswers = score.Responses.Count(r => r.IsCorrect),
                TotalQuestions = materialQuestions.Count,
                SubmittedAt = score.CreatedAt
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error submitting test answers");
            throw;
        }
    }

// Новый метод для обработки ответа
    private async Task ProcessResponse(Question question, UserAnswerDto answer, Score score)
    {
        bool isCorrect = CheckAnswer(question, answer);
        int points = isCorrect ? question.Points : 0;

        var response = new Response
        {
            ScoreId = score.Id,
            UserId = score.UserId, // Добавляем userId
            QuestionId = answer.QuestionId,
            UserAnswer = answer.TextAnswer,
            IsCorrect = isCorrect,
            Points = points
        };

        await _responseRepository.CreateAsync(response);

        if (answer.SelectedOptionIds?.Any() == true)
        {
            var responseOptions = answer.SelectedOptionIds.Select(optionId =>
                new ResponseOption
                {
                    ResponseId = response.Id,
                    OptionId = optionId
                });

            await _responseOptionRepository.AddRangeAsync(responseOptions);
        }

        score.Score1 += points;
    }

    public async Task<TestResultDto> GetTestResultAsync(int scoreId)
    {
        var score = await _scoreRepository.GetQueryable()
            .Include(s => s.User)
            .Include(s => s.Responses)
            .ThenInclude(r => r.Question)
            .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
            .ThenInclude(r => r.ResponseOptions)
            .ThenInclude(ro => ro.Option)
            .Include(s => s.Responses)
            .ThenInclude(r => r.Question)
            .ThenInclude(q => q.CorrectAnswers)
            .FirstOrDefaultAsync(s => s.Id == scoreId);

        if (score == null) 
            throw new KeyNotFoundException("Test attempt not found");

        var totalQuestions = await _questionRepository.GetQueryable()
            .CountAsync(q => q.LearnMaterialId == score.LearnMaterialId);

        return new TestResultDto
        {
            ScoreId = score.Id,
            TotalScore = score.Score1,
            CorrectAnswers = score.Responses.Count(r => r.IsCorrect),
            TotalQuestions = totalQuestions,
            SubmittedAt = score.CreatedAt,
            Responses = score.Responses.Select(r => new UserQuestionResponseDto
            {
                QuestionId = r.QuestionId,
                QuestionText = r.Question.QuestionText,
                QuestionType = r.Question.QuestionType,
                UserAnswer = r.UserAnswer,
                IsCorrect = r.IsCorrect,
                Points = r.Points,
                SelectedOptions = r.ResponseOptions.Select(ro => new OptionDto
                {
                    Id = ro.Option.Id,
                    Text = ro.Option.OptionText,
                    IsCorrect = r.Question.CorrectAnswers
                        .Any(ca => ca.OptionId == ro.Option.Id)
                }).ToList()
            }).ToList()
        };
    }

    public async Task<IEnumerable<UserAttemptDto>> GetUserAttemptsForMaterialAsync(int userId, int materialId)
    {
        var materialExists = await _context.LearnMaterials.AnyAsync(lm => lm.Id == materialId);
        if (!materialExists)
            throw new KeyNotFoundException("Учебный материал не найден");

        var attempts = await _context.Scores
            .Include(s => s.User)
            .Include(s => s.Responses)
            .ThenInclude(r => r.Question)
            .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
            .ThenInclude(r => r.ResponseOptions)
            .ThenInclude(ro => ro.Option)
            .Where(s => s.LearnMaterialId == materialId && s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return attempts.Select(attempt => new UserAttemptDto
        {
            UserId = attempt.UserId,
            UserName = attempt.User.FullName,
            TotalScore = attempt.Score1,
            Photo = attempt.User.Photo,
            AttemptDate = attempt.CreatedAt,
            Responses = attempt.Responses.Select(r => new UserQuestionResponseDto
            {
                QuestionId = r.QuestionId,
                QuestionText = r.Question.QuestionText,
                QuestionType = r.Question.QuestionType,
                UserAnswer = r.UserAnswer,
                IsCorrect = r.IsCorrect,
                Points = r.Points,
                SelectedOptions = r.ResponseOptions.Select(ro => new OptionDto
                {
                    Id = ro.Option.Id,
                    Text = ro.Option.OptionText,
                    IsCorrect = r.Question.CorrectAnswers
                        .Any(ca => ca.OptionId == ro.Option.Id)
                }).ToList()
            }).ToList()
        });
    }
    
    public async Task<IEnumerable<UserAttemptDto>> GetAllResponsesForMaterialAsync(int materialId)
    {
        var materialExists = await _context.LearnMaterials.AnyAsync(lm => lm.Id == materialId);
        if (!materialExists)
        {
            throw new KeyNotFoundException("Учебный материал не найден");
        }

        var attempts = await _context.Scores
            .Include(s => s.User)
            .Include(s => s.Responses)
            .ThenInclude(r => r.Question)
            .ThenInclude(q => q.Options)
            .Include(s => s.Responses)
            .ThenInclude(r => r.ResponseOptions)
            .ThenInclude(ro => ro.Option)
            .Where(s => s.LearnMaterialId == materialId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return attempts.Select(attempt => new UserAttemptDto
        {
            UserId = attempt.UserId,
            UserName = attempt.User.FullName,
            TotalScore = attempt.Score1,
            Photo = attempt.User.Photo,
            AttemptDate = attempt.CreatedAt,
            Responses = attempt.Responses.Select(r => new UserQuestionResponseDto
            {
                QuestionId = r.QuestionId,
                QuestionText = r.Question.QuestionText,
                QuestionType = r.Question.QuestionType,
                UserAnswer = r.UserAnswer,
                IsCorrect = r.IsCorrect,
                Points = r.Points,
                SelectedOptions = r.ResponseOptions
                    .Select(ro => new OptionDto
                    {
                        Id = ro.Option.Id,
                        Text = ro.Option.OptionText,
                        IsCorrect = r.Question.CorrectAnswers
                            .Any(ca => ca.OptionId == ro.Option.Id)
                    })
                    .ToList()
            }).ToList()
        });
    }

    private bool CheckAnswer(Question question, UserAnswerDto answer)
    {
        try
        {
            return question.QuestionType switch
            {
                "Text" => CheckTextAnswer(question, answer.TextAnswer),
                "RadioButton" => CheckSingleChoiceAnswer(question, answer.SelectedOptionIds),
                "CheckBox" => CheckMultipleChoiceAnswer(question, answer.SelectedOptionIds),
                _ => throw new InvalidOperationException($"Unknown question type: {question.QuestionType}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking answer for question {question.Id}");
            return false;
        }
    }

    private bool CheckTextAnswer(Question question, string userAnswer)
    {
        return string.Equals(
            question.CorrectAnswer?.Trim(),
            userAnswer?.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    private bool CheckSingleChoiceAnswer(Question question, List<int> selectedOptions)
    {
        if (selectedOptions.Count != 1) return false;

        var correctAnswer = question.CorrectAnswers.FirstOrDefault();
        return correctAnswer != null && selectedOptions[0] == correctAnswer.OptionId;
    }

    private bool CheckMultipleChoiceAnswer(Question question, List<int> selectedOptions)
    {
        var correctOptionIds = question.CorrectAnswers.Select(ca => ca.OptionId).ToList();
        return selectedOptions.Count == correctOptionIds.Count &&
               selectedOptions.All(id => correctOptionIds.Contains(id));
    }
}