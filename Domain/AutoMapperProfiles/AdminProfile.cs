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
            CreateMap<CustomerAddModel, Customer>();
            CreateMap<CustomerUpdateModel, Customer>();
            CreateMap<Customer, CustomerResponseModel>();

            CreateMap<FruitAddModel, Fruit>();
            CreateMap<Fruit, FruitResponseModel>();

            CreateMap<SupplyAddModel, Supply>();
            CreateMap<SupplyUpdateModel, Supply>();
            CreateMap<SupplyResponseModel, Supply>()
                .ForMember(dest => dest.Fruit, opt => opt.Ignore())
                .ForMember(dest => dest.Truck, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.TruckAssignment, opt => opt.Ignore());

            CreateMap<Supply, SupplyResponseModel>()
                .ForMember(dest => dest.FruitName, opt => opt.MapFrom(src => src.Fruit.Name))
                .ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src => src.Truck.TruckNumber))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));

            CreateMap<OrderAddModel, Order>();
            CreateMap<OrderUpdateModel, Order>();
            CreateMap<OrderResponseModel, Order>()
                .ForMember(dest => dest.Fruit, opt => opt.Ignore())
                .ForMember(dest => dest.Truck, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.TruckAssignment, opt => opt.Ignore());

            CreateMap<Order, OrderResponseModel>()
                .ForMember(dest => dest.FruitName, opt => opt.MapFrom(src => src.Fruit.Name))
                .ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src => src.Truck.TruckNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));
        }
    }
}
