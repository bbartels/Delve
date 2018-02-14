using AutoMapper;
using Delve.Demo.Dto;
using Delve.Demo.Models;

namespace Delve.Demo
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserRole, UserRoleDto>();
        }
    }
}
