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

        #region Client
        public async Task<Client> AddClient(Client clientEntity)
        {
            var client = _context.Client.Add(clientEntity);
            return await Task.FromResult(client.Entity);
        }
        public async Task<List<Client>> GetAllClients()
        {
            var clients = await _context.Set<Client>().ToListAsync();
            return clients;
        }

        public async Task<Client> GetClient(int clientId)
        {
            var client = await _context.Set<Client>().FirstOrDefaultAsync(x => x.Id == clientId && x.IsActive == true);
            return client;
        }

        public async Task<Client> GetClientByName(string clientName)
        {
            var client = await _context.Set<Client>().FirstOrDefaultAsync(x => x.Name == clientName);
            return client;
        }


        #endregion


        #region Consumer
        public async Task<Consumer> AddConsumer(Consumer consumerEntity)
        {
            var consumer = _context.Consumer.Add(consumerEntity);
            return await Task.FromResult(consumer.Entity);
        }
        public async Task<List<Consumer>> GetAllConsumers()
        {
            var consumers = await _context.Set<Consumer>().ToListAsync();
            return consumers;
        }
        public async Task<Consumer> GetConsumer(int consumerId)
        {
            var consumer = await _context.Set<Consumer>().FirstOrDefaultAsync(x => x.Id == consumerId && x.IsActive == true);
            return consumer;
        }
        public async Task<Consumer> GetConsumerForUpdate(int consumerId)
        {
            var consumer = await _context.Set<Consumer>().FirstOrDefaultAsync(x => x.Id == consumerId);
            return consumer;
        }

        public async Task<Consumer> GetConsumerByName(string consumerName)
        {
            var consumer = await _context.Set<Consumer>().FirstOrDefaultAsync(x => x.FirstName == consumerName);
            return consumer;
        }
        #endregion

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


        #endregion
    }
}
