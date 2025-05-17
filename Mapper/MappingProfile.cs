using AutoMapper;
using KaznacheystvoCourse.DTO.Option;
using KaznacheystvoCourse.DTO.Question;
using KaznacheystvoCourse.DTO.User;
using KaznacheystvoCourse.Models;

namespace KaznacheystvoCourse.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //User Mappers
        CreateMap<CreateUserDTO, User>();
        CreateMap<User, GetAllUserDTO>();
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
    }
}