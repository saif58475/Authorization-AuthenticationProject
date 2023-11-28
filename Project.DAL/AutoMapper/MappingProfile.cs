using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project.DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IdentityUser, CreateUserDto>().ReverseMap();
            CreateMap<IdentityUser, UpdateUserDto>().ReverseMap();
            CreateMap<IdentityUser, LoginUser>().ReverseMap();
        }
    }
}
