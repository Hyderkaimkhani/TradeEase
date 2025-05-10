using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        #region Customer
        Task<ResponseModel<CustomerResponseModel>> AddCustomer(CustomerAddModel customerModel);

        Task<ResponseModel<CustomerResponseModel>> GetCustomer(int customerId);

        Task<ResponseModel<List<CustomerResponseModel>>> GetAllCustomers();

        Task<ResponseModel<List<CustomerResponseModel>>> GetCustomers(bool isActive);

        Task<List<DropDownModel>> GetCustomersDropDown();

        Task<ResponseModel<CustomerResponseModel>> UpdateCustomer(CustomerAddModel customerModel);

        Task<ResponseModel<string>> DeleteCustomer(int customerId);
        #endregion

        #region Fruit

        Task<ResponseModel<FruitResponseModel>> AddFruit(FruitAddModel requestModel);

        Task<ResponseModel<FruitResponseModel>> GetFruit(int FruitId);

        Task<ResponseModel<List<FruitResponseModel>>> GetFruits();

        Task<ResponseModel<FruitResponseModel>> UpdateFruit(FruitAddModel FruitModel);

        #endregion
    }
}
