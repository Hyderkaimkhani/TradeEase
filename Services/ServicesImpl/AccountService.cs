using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.ServicesImpl
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IMapper mapper;

        public AccountService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
        }

        public async Task<ResponseModel<AccountResponseModel>> AddAccount(AccountAddModel model)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<AccountResponseModel>();
                var account = mapper.Map<Account>(model);

                account.IsActive = true;
                var added = await unitOfWork.AccountRepository.AddAccount(account);

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = mapper.Map<AccountResponseModel>(added);
                    response.Message = "Account created successfully";
                }
                return response;
            }
        }

        public async Task<ResponseModel<AccountResponseModel>> GetAccount(int id)
        {
            var response = new ResponseModel<AccountResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var Account = await unitOfWork.AccountRepository.GetAccount(id);
                if (Account == null)
                    return new ResponseModel<AccountResponseModel> { IsError = true, Message = "Account not found" };

                response.Model = mapper.Map<AccountResponseModel>(Account);
                return response;
            }
        }

        public async Task<PaginatedResponseModel<AccountResponseModel>> GetAccounts(FilterModel filterModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<AccountResponseModel>();
                var accounts = await unitOfWork.AccountRepository.GetAccounts(filterModel);
                if (accounts.Model == null || accounts.Model.Count < 1)
                {
                    response.Message = "No Order found";
                    response.Model = new List<AccountResponseModel>();
                }
                else
                {
                    response.Model = mapper.Map<List<AccountResponseModel>>(accounts);
                }

                return response;
            }
        }

        public async Task<ResponseModel<AccountResponseModel>> UpdateAccount(AccountUpdateModel model)
        {
            var response = new ResponseModel<AccountResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var Account = await unitOfWork.AccountRepository.GetAccount(model.Id);
                if (Account == null)
                {
                    response.Message = "Account not found";
                    response.IsError = true;
                    return response;
                }

                Account.Name = model.Name;
                Account.Type = model.Type;
                Account.AccountNumber = model.AccountNumber;
                Account.BankName = model.BankName;

                // AccountTransaction entry if balance changed
                //Account.CurrentBalance = model.CurrentBalance;

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = mapper.Map<AccountResponseModel>(Account);
                    response.Message = "Account updated successfully";
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> DeleteAccount(int id)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var Account = await unitOfWork.AccountRepository.GetAccount(id);
                if (Account == null)
                {
                    response.Message = "Account not found";
                    response.IsError = true;
                    return response;
                }
                Account.IsActive = false;
                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = true;
                    response.Message = "Account deleted successfully";
                }
                return response;
            }
        }

    }
}
