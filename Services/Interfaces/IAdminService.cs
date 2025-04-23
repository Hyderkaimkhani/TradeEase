using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        #region Client
        Task<ResponseModel<ClientModel>> AddClient(ClientModel clientModel);

        Task<ResponseModel<ClientModel>> GetClient(int clientId);

        Task<List<ClientModel>> GetAllClients();

        Task<ResponseModel<ClientModel>> UpdateClient(ClientModel clientModel);

        Task<ResponseModel<string>> DeleteClient(int clientId);
        #endregion

        #region Consumer
        Task<ResponseModel<ConsumerModel>> AddConsumer(ConsumerModel consumerModel);
        Task<ResponseModel<ConsumerModel>> GetConsumer(int consumerId);

        Task<List<ConsumerModel>> GetAllConsumers();

        Task<ResponseModel<ConsumerModel>> UpdateConsumer(ConsumerModel consumerModel);

        Task<ResponseModel<string>> DeleteConsumer(int consumerId);
        #endregion

        #region Customer
        Task<ResponseModel<CustomerModel>> AddCustomer(CustomerModel customerModel);

        Task<ResponseModel<CustomerModel>> GetCustomer(int customerId);

        Task<List<CustomerModel>> GetAllCustomers();

        Task<ResponseModel<CustomerModel>> UpdateCustomer(CustomerModel customerModel);

        Task<ResponseModel<string>> DeleteCustomer(int customerId);
        #endregion
    }
}
