using AutoMapper;
using backend.Core.Dtos.ToDo;
using backend.Core.Dtos.User;
using backend.Core.Models;

namespace backend.Core.AutoMapperConfigProfile
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            // User
            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.HashPassword, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

            // ToDo
            CreateMap<ToDoCreateDto, ToDo>();
            CreateMap<ToDo, ToDoGetDto>();
        }
    }
}
