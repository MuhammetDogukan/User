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
            CreateMap<CreateUser, User>();
            CreateMap<User, CreateUser>();
            CreateMap<User, GetUser>();
            CreateMap<UpdateUser, User>();
        }
    }
}
