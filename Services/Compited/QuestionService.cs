using AutoMapper;
using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.DTO.Question;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class QuestionService : IQuestionService
{
    private readonly IGenericRepository<Question> _questionRepo;
    private readonly IGenericRepository<Option> _optionRepo;
    private readonly IGenericRepository<CorrectAnswer> _correctAnswerRepo;
    private readonly IGenericRepository<LearnMaterial> _materialRepo;
    private readonly IMapper _mapper;

    public QuestionService(
        IGenericRepository<Question> questionRepo,
        IGenericRepository<Option> optionRepo,
        IGenericRepository<CorrectAnswer> correctAnswerRepo,
        IGenericRepository<LearnMaterial> materialRepo, IMapper mapper)
    {
        _questionRepo = questionRepo;
        _optionRepo = optionRepo;
        _correctAnswerRepo = correctAnswerRepo;
        _materialRepo = materialRepo;
        _mapper = mapper;
    }

    public async Task<QuestionDto> GetQuestionAsync(int id)
    {
        var question = await _questionRepo.GetQueryable()
            .Include(q => q.Options)
            .Include(q => q.CorrectAnswers)
            .Include(x => x.LearnMaterial)
            .FirstOrDefaultAsync(q => q.Id == id);

        return _mapper.Map<QuestionDto>(question);
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestionsByMaterialAsync(int materialId)
    {
        var questions = await _questionRepo.GetQueryable()
            .Where(q => q.LearnMaterialId == materialId)
            .Include(q => q.Options)
            .Include(q => q.CorrectAnswers)
            .ToListAsync();

        return _mapper.Map<List<QuestionDto>>(questions);
    }

    public async Task<QuestionDto> CreateQuestionAsync(int materialId, CreateQuestionDto dto)
    {
        ValidateQuestionDto(dto);
        await ValidateMaterialExists(materialId);

        // Create question
        var question = new Question
        {
            QuestionText = dto.QuestionText,
            QuestionType = dto.QuestionType,
            CorrectAnswer = dto.QuestionType == "Text" ? dto.CorrectAnswer : null,
            LearnMaterialId = materialId,
            Points = dto.Points
        };

        await _questionRepo.CreateAsync(question);

        // Process options
        if (dto.QuestionType != "Text")
        {
            var options = dto.Options.Select(o => new Option
            {
                QuestionId = question.Id,
                OptionText = o.Text
            }).ToList();

            await _optionRepo.AddRangeAsync(options);

            // Process correct answers
            var correctOptions = dto.Options
                .Select((o, index) => new { o.IsCorrect, Index = index })
                .Where(x => x.IsCorrect)
                .Select(x => options[x.Index].Id)
                .ToList();

            var correctAnswers = correctOptions.Select(id => new CorrectAnswer
            {
                QuestionId = question.Id,
                OptionId = id
            }).ToList();

            await _correctAnswerRepo.AddRangeAsync(correctAnswers);
        }

        return await GetQuestionAsync(question.Id);
    }

    // Остальные методы (Update, Delete) аналогично с проверками

    public async Task<QuestionDto> UpdateQuestionAsync(int id, UpdateQuestionDto dto)
    {
        ValidateQuestionDto(dto);
        var question = await _questionRepo.GetQueryable()
            .Include(q => q.Options)
            .Include(q => q.CorrectAnswers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null) throw new KeyNotFoundException("Question not found");

        // Update main fields
        question.QuestionText = dto.QuestionText;
        question.QuestionType = dto.QuestionType;
        question.CorrectAnswer = dto.QuestionType == "Text" ? dto.CorrectAnswer : null;
        question.Points = dto.Points;

        // Delete old options and answers
        await _correctAnswerRepo.DeleteRangeAsync(question.CorrectAnswers);
        await _optionRepo.DeleteRangeAsync(question.Options);

        // Process new options
        if (dto.QuestionType != "Text")
        {
            var options = dto.Options.Select(o => new Option
            {
                QuestionId = question.Id,
                OptionText = o.Text
            }).ToList();

            await _optionRepo.AddRangeAsync(options);

            // Process new correct answers
            var correctOptions = dto.Options
                .Select((o, index) => new { o.IsCorrect, Index = index })
                .Where(x => x.IsCorrect)
                .Select(x => options[x.Index].Id)
                .ToList();

            var correctAnswers = correctOptions.Select(id => new CorrectAnswer
            {
                QuestionId = question.Id,
                OptionId = id
            }).ToList();

            await _correctAnswerRepo.AddRangeAsync(correctAnswers);
        }

        await _questionRepo.UpdateAsync(question);

        return await GetQuestionAsync(question.Id);
    }

    public async Task DeleteQuestionAsync(int id)
    {
        var question = await _questionRepo.GetByIdAsync(id);
        if (question != null)
        {
            await _questionRepo.DeleteAsync(question);
        }
    }

    private void ValidateQuestionDto(CreateQuestionDto dto)
    {
        if (dto.QuestionType == "Text")
        {
            if (string.IsNullOrEmpty(dto.CorrectAnswer))
                throw new ArgumentException("Correct answer is required for Text questions");

            if (dto.Options.Any())
                throw new ArgumentException("Options are not allowed for Text questions");
        }
        else
        {
            if (dto.Options.Count < 2)
                throw new ArgumentException("At least two options are required");

            var correctCount = dto.Options.Count(o => o.IsCorrect);

            if (correctCount == 0)
                throw new ArgumentException("At least one correct answer is required");

            if (dto.QuestionType == "RadioButton" && correctCount > 1)
                throw new ArgumentException("RadioButton questions can have only one correct answer");
        }
    }

    private async Task ValidateMaterialExists(int materialId)
    {
        if (!await _materialRepo.GetQueryable().AnyAsync(m => m.Id == materialId))
            throw new ArgumentException("Learn material not found");
    }
}