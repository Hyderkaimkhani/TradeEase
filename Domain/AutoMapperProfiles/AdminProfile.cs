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
            CreateMap<CompanyAddRequestModel, Company>()
                .ForMember(dest => dest.Logo, opt => opt.Ignore());


            CreateMap<CompanyUpdateRequestModel, Company>();
            CreateMap<Company, CompanyResponseModel>();

            CreateMap<AccountAddModel, Account>();
            CreateMap<AccountUpdateModel, Account>();
            CreateMap<Account, AccountResponseModel>();


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
                //.ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src => src.Truck.TruckNumber))
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
                //.ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src => src.Truck.TruckNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));

            CreateMap<Payment, PaymentResponseModel>()
               .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
               .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.Name));

            CreateMap<PaymentAllocation, PaymentAllocationResponseModel>();

            CreateMap<Bill, BillResponseModel>()
                .ForMember(dest => dest.BillDate, opt => opt.MapFrom(src => src.CreatedDate));
            CreateMap<BillDetail, BillDetailResponseModel>()
                .ForMember(dest => dest.RecordDate, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.OrderDate : src.Supply.SupplyDate))

            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.Status : string.Empty))

            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.PaymentStatus : src.Supply.PaymentStatus))

            .ForMember(dest => dest.TruckNumber, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.TruckNumber : src.Supply.TruckNumber))

            .ForMember(dest => dest.FruitName, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.Fruit.Name : src.Supply.Fruit.Name))

            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
                src.Order != null ? src.Order.Fruit.UnitType : src.Supply.Fruit.UnitType));
            CreateMap<BillAddRequestModel, Bill>();

        }
    }
}
