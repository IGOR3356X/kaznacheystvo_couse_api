using AutoMapper;
using KaznacheystvoCalendar.Interfaces;
using KaznacheystvoCourse.DTO;
using KaznacheystvoCourse.DTO.Authorization;
using KaznacheystvoCourse.DTO.User;
using KaznacheystvoCourse.Interfaces.ISevices;
using KaznacheystvoCourse.Models;
using Microsoft.EntityFrameworkCore;

namespace KaznacheystvoCalendar.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _repository;
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    
    public UserService(IGenericRepository<User> repository, IMapper mapper, IS3Service s3Service)
    {
        _repository = repository;
        _mapper = mapper;
        _s3Service = s3Service;
    }
    public async Task<PaginatedResponse<GetAllUserDTO>> GetUsersAsync(QueryObject query)
    {
        var users = _repository.GetQueryable();
        
        if (!string.IsNullOrEmpty(query.Search))
        {
            // Применяем фильтрацию по всем полям, которые вы хотите включить в поиск
            var searchLower = query.Search.ToLower();
            users = users.Where(r =>
                r.Id.ToString().ToLower().Contains(searchLower) ||
                r.Login.ToLower().Contains(searchLower) ||
                r.FullName.ToLower().Contains(searchLower)
            );
        }
        
        var skipNumber = (query.PageNumber - 1) * query.PageSize;
        
        
        var items = await users
            .OrderByDescending(e => e.FullName)
            .Skip(skipNumber)
            .Take(query.PageSize)
            .ToListAsync();
        
        var totalCount = await users.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        
        var result = new PaginatedResponse<GetAllUserDTO>
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = _mapper.Map<List<GetAllUserDTO>>(items)
        };
        
        return result;
    }

    public async Task<GetUserDTO> GetUserByIdAsync(int id)
    {
        return _mapper.Map<GetUserDTO>(await _repository.GetQueryable()
            .Include(x => x.Role)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync());
    }

    public async Task<User?> CreateUserAsync(CreateUserDTO createUserDto)
    {
        if (await _repository.GetQueryable().Where(x => x.Login.Contains(createUserDto.Login)).AnyAsync())
        {
            return null;
        };
        return await _repository.CreateAsync(_mapper.Map<User>(createUserDto));
    }

    public async Task<UserServicesErrors> UpdateUserAsync(int id, UpdateUserDTO updateUserDto)
    {
        string? photo = null;
        
        var user = await _repository.GetByIdAsync(id);
        if(user == null)
            return UserServicesErrors.NotFound;
        
        if (await _repository.GetQueryable().Where(x => x.Login.Contains(updateUserDto.Login) && x.Id != id).FirstOrDefaultAsync() != null)
        {
            return UserServicesErrors.AlreadyExists;
        }
        
        if (updateUserDto.Photo != null && updateUserDto.Photo.Length > 0)
        {
            photo = await _s3Service.UploadFileAsync(updateUserDto.Photo, id);
        }
        
        user.Login = updateUserDto.Login ?? user.Login;
        user.Password = updateUserDto.Password ?? user.Password;
        user.FullName = updateUserDto.FullName ?? user.FullName;
        user.Email = updateUserDto.Email ?? user.Email;
        user.Photo = photo ?? user.Photo;
        user.RoleId = updateUserDto.RoleId ?? user.RoleId;
        
        
        await _repository.UpdateAsync(user);
        
        return UserServicesErrors.Ok;
    }
    
    public async Task<bool> DeleteUserAsync(int id)
    {
        var isExists = await _repository.GetByIdAsync(id);
        if (isExists == null) return false;
        await _repository.DeleteAsync(isExists);
        return true;
    }

    public async Task<User> LoginUserAsync(AuthDTO authDTO)
    {
        return await _repository.GetQueryable()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Login.Contains(authDTO.Login) && x.Password.Contains(authDTO.Password));
    }
}