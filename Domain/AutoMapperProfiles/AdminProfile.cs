using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Domain.AutoMapperProfiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<CustomerAddModel, Customer>().ReverseMap();
            CreateMap<Customer, CustomerResponseModel>().ReverseMap();
        }
    }
}
