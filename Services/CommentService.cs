using AutoMapper;
using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO.Comment;
using KaznacheystvoCourse.DTO.User;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class CommentService : ICommentService
{
    private readonly IGenericRepository<Comment> _commentRepository;
    private readonly IGenericRepository<LearnMaterial> _materialRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IMapper _mapper;
    
    public CommentService(
        IGenericRepository<Comment> commentRepository,
        IGenericRepository<LearnMaterial> materialRepository,
        IGenericRepository<User> userRepository,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _materialRepository = materialRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<CommentDto>> GetCommentsForMaterialAsync(int learnMaterialId)
    {
        var materialExists = await _materialRepository.GetQueryable().AnyAsync(lm => lm.Id == learnMaterialId);
        if (!materialExists)
            throw new KeyNotFoundException("Учебный материал не найден");
        
        var comments = await _commentRepository.GetQueryable()
            .Where(c => c.LearnMaterialId == learnMaterialId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
    
    public async Task<CommentDto> CreateCommentAsync(int userId, CreateCommentDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId) 
            ?? throw new KeyNotFoundException("Пользователь не найден");
        
        var materialExists = await _materialRepository.GetQueryable().AnyAsync(lm => lm.Id == dto.LearnMaterialId);
        if (!materialExists)
            throw new KeyNotFoundException("Учебный материал не найден");
    
        var comment = new Comment
        {
            UserId = userId,
            LearnMaterialId = dto.LearnMaterialId,
            Text = dto.Text,
            CreatedDateTime = DateTime.Now
        };
    
        await _commentRepository.CreateAsync(comment);
        
        return new CommentDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedDateTime = comment.CreatedDateTime,
            Author = _mapper.Map<UserInfoDto>(user)
        };
    }
    
    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _commentRepository.GetQueryable()
            .FirstOrDefaultAsync(c => c.Id == commentId)
            ?? throw new KeyNotFoundException("Комментарий не найден");
    
        await _commentRepository.DeleteAsync(comment);
    }
}