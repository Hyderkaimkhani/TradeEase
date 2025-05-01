using Domain.Entities;
using Repositories.Context;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.RepositoriesImpl
{
    public class AdminRepository : IAdminRepository
    {
        private readonly Context.Context _context;

        internal AdminRepository(Context.Context context)
        {
            _context = context;
        }

        #region Customer
        public async Task<Customer> AddCustomer(Customer customerEntity)
        {
            var Customer = _context.Customer.Add(customerEntity);
            return await Task.FromResult(Customer.Entity);
        }
        public async Task<List<Customer>> GetAllCustomers()
        {
            var Customers = await _context.Set<Customer>().ToListAsync();
            return Customers;
        }

        public async Task<Customer> GetCustomer(int customerId)
        {
            var Customer = await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Id == customerId);
            return Customer;
        }

        public async Task<Customer> GetCustomerByName(string customerName)
        {
            var Customer = await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Name == customerName);
            return Customer;
        }

        public async Task<List<Customer>> GetCustomers(bool isActive)
        {
            var Customers = await _context.Set<Customer>().Where(x => x.IsActive == isActive).ToListAsync();
            return Customers;
        }

        #endregion
    }
}
