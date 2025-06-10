using AutoMapper;
using UserCrud.Models;
using UserCrud.Models.Dto;

namespace UserCrud.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>(); 
            CreateMap<CreateUserDto, User>();
        }
    }
}
