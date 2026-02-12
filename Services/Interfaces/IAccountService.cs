using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task<ResponseModel<AccountResponseModel>> AddAccount(AccountAddModel model);
        Task<ResponseModel<AccountResponseModel>> GetAccount(int id);
        Task<PaginatedResponseModel<AccountResponseModel>> GetAccounts(FilterModel filterModel);
        Task<ResponseModel<AccountResponseModel>> UpdateAccount(AccountUpdateModel model);
        Task<ResponseModel<bool>> DeleteAccount(int id);
        Task<List<DropDownModel>> GetAccountsDropDown();
        Task<List<AccountResponseModel>> GetAccountsByType(string type);
    }
}
