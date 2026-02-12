using AutoMapper;
using Domain.Entities;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Domain.AutoMapperProfiles
{
    public class AccountTransactionProfile : Profile
    {
        public AccountTransactionProfile()
        {
            // AccountTransactionAddModel -> AccountTransaction
            CreateMap<AccountTransactionAddModel, AccountTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // AccountTransaction -> AccountTransactionResponseModel
            CreateMap<AccountTransaction, AccountTransactionResponseModel>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : ""))
                .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : ""))
                .ForMember(dest => dest.ToAccountName, opt => opt.MapFrom(src => src.ToAccount != null ? src.ToAccount.Name : ""));
            //.ForMember(dest => dest.SignedAmount, opt => opt.MapFrom(src => 
            //    src.TransactionDirection == "Debit" ? src.Amount : src.Amount));

            //CreateMap<AccountTransaction, PaymentResponseModel>()
            //    .ForMember(dest => dest.TransactionFlow, opt => opt.MapFrom(src =>
            //    src.TransactionDirection == "Debit" ? "Received" : "Paid"))
            //    .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : ""))
            //    .ForMember(dest => dest.PaymentBy, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : ""));
        }
    }
}