using AutoMapper;
using KaznacheystvoCourse.DTO.Comment;
using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.DTO.Question;
using KaznacheystvoCourse.DTO.User;
using KaznacheystvoCourse.DTO.UserCourses;
using KaznacheystvoCourse.Models;

namespace KaznacheystvoCourse.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //User Mappers
        CreateMap<CreateUserDTO, User>();
        CreateMap<User, GetAllUserDTO>()
            .ForMember(x=> x.Role, opt => opt
                .MapFrom(x=> x.Role.Name));
        CreateMap<User, GetUserDTO>()
            .ForMember(act => act.Role, opt => opt
                .MapFrom(src => src.Role.Name));
        CreateMap<UpdateUserDTO, User>();
        
        //Question Mappers
        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.OptionText,
                IsCorrect = src.CorrectAnswers.Any(ca => ca.OptionId == o.Id)
            })));

        CreateMap<CreateQuestionDto, Question>();
        CreateMap<UpdateQuestionDto, Question>();
        
        //Comment Mappers
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));
        CreateMap<User, UserInfoDto>();
        
        //UserCourses Mappers
        CreateMap<Course, MaterialDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.Ispublish));
    }
}