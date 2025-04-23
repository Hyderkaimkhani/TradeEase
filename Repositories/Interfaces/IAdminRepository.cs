using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAdminRepository
    {
        #region Client
        Task<Client> AddClient(Client clientEntity);
        Task<Client> GetClient(int clientId);
        Task<List<Client>> GetAllClients();
        Task<Client> GetClientByName(string clientName);
        #endregion

        #region Consumer
        Task<Consumer> AddConsumer(Consumer consumerEntity);
        Task<Consumer> GetConsumer(int consumerId);
        Task<Consumer> GetConsumerForUpdate(int consumerId);
        Task<List<Consumer>> GetAllConsumers();
        Task<Consumer> GetConsumerByName(string consumerName);
        #endregion

        #region Customer
        Task<Customer> AddCustomer(Customer customerEntity);
        Task<Customer> GetCustomer(int customerId);
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerByName(string customerName);
        #endregion
    }
}
