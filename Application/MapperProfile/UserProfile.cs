using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.DtoEntity;
using Domain.Entity;

namespace Domain.MapperProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<User, CreateUserDto>();
            CreateMap<User, GetUserDto>();
            CreateMap<GetUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
