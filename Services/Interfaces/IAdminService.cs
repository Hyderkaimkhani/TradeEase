using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        #region Customer
        Task<ResponseModel<CustomerResponseModel>> AddCustomer(CustomerAddModel customerModel);

        Task<ResponseModel<CustomerResponseModel>> GetCustomer(int customerId);

        Task<ResponseModel<List<CustomerResponseModel>>> GetAllCustomers();

        Task<ResponseModel<List<CustomerResponseModel>>> GetCustomers(bool isActive);

        Task<ResponseModel<CustomerResponseModel>> UpdateCustomer(CustomerAddModel customerModel);

        Task<ResponseModel<string>> DeleteCustomer(int customerId);
        #endregion
    }
}
