using AutoMapper;
using Domain.Entities;
using Domain.Models;

namespace Domain.AutoMapperProfiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<ClientModel, Client>().ReverseMap();
            CreateMap<Client, ClientModel>().ReverseMap();
            CreateMap<CustomerModel, Customer>().ReverseMap();
            CreateMap<Customer, CustomerModel>().ReverseMap();
            CreateMap<Consumer, ConsumerModel>().ReverseMap();
        }
    }
}
