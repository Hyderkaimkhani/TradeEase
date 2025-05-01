using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAdminRepository
    {
        #region Customer
        Task<Customer> AddCustomer(Customer customerEntity);
        Task<Customer> GetCustomer(int customerId);
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerByName(string customerName);

        Task<List<Customer>> GetCustomers(bool isActive);
        #endregion
    }
}
