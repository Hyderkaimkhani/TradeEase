using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Repositories.Interfaces;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        #region Customer
        Task<ResponseModel<CustomerResponseModel>> AddCustomer(CustomerAddModel customerModel);

        Task<ResponseModel<CustomerResponseModel>> GetCustomer(int customerId);

        Task<ResponseModel<List<CustomerResponseModel>>> GetAllCustomers();

        Task<ResponseModel<List<CustomerResponseModel>>> GetCustomers(bool? isActive, string? entityType);

        Task<List<DropDownModel>> GetCustomersDropDown(string? entityType);

        Task<ResponseModel<CustomerResponseModel>> UpdateCustomer(CustomerUpdateModel customerModel);

        Task<ResponseModel<string>> DeleteCustomer(int customerId);

        Task AdjustCustomerBalance(IUnitOfWork unitOfWork, int customerId, decimal oldAmount, decimal newAmount, string type);
        Customer AdjustCustomerBalance(Customer customer, decimal oldAmount, decimal newAmount, string type);
        #endregion

        #region Fruit

        Task<ResponseModel<FruitResponseModel>> AddFruit(FruitAddModel requestModel);

        Task<ResponseModel<FruitResponseModel>> GetFruit(int FruitId);

        Task<ResponseModel<List<FruitResponseModel>>> GetFruits();

        Task<ResponseModel<FruitResponseModel>> UpdateFruit(FruitAddModel FruitModel);

        #endregion
    }
}
