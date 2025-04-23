using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;

namespace Domain.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, User>().ReverseMap();
            CreateMap<AddUserRequestModel, User>().ReverseMap();
            CreateMap<AppTokenRequestModel, AppToken>().ReverseMap();
        }
    }
}
