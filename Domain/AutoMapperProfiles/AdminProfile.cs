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

            CreateMap<FruitAddModel, Fruit>().ReverseMap();
            CreateMap<Fruit, FruitResponseModel>().ReverseMap();

            CreateMap<SupplyAddRequest, Supply>().ReverseMap();
            CreateMap<SupplyUpdateRequest, Supply>().ReverseMap();
            CreateMap<SupplyResponseModel, Supply>()
                .ForMember(dest => dest.Fruit, opt => opt.Ignore())
                .ForMember(dest => dest.Truck, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.TruckAssignment, opt => opt.Ignore());

            CreateMap<Supply, SupplyResponseModel>()
                .ForMember(dest => dest.Fruit, opt => opt.MapFrom(src => src.Fruit.Name))
                .ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src => src.Truck.TruckNumber))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier.Name));
        }
    }
}
