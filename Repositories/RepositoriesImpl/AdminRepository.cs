using Domain.Entities;
using Domain.Models.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

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
            var Customers = await _context.Set<Customer>().AsNoTracking().ToListAsync();
            return Customers;
        }

        public async Task<Customer?> GetCustomer(int customerId)
        {
            var Customer = await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Id == customerId);
            return Customer;
        }

        public async Task<Customer?> GetCustomerByName(string customerName)
        {
            var Customer = await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Name == customerName);
            return Customer;
        }

        public async Task<List<Customer>> GetCustomers(bool? isActive)
        {
            var query = _context.Set<Customer>().AsNoTracking();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var customers = await query.ToListAsync();

            return customers;
        }

        public async Task<List<DropDownModel>> GetCustomersDropDown()
        {
            var query = _context.Set<Customer>().AsNoTracking();

            var customers = await query.Where(c => c.IsActive)
            .Select(c => new DropDownModel
            {
                Key = c.Id,
                Value = c.Name
            })
            .OrderBy(c => c.Value)
            .ToListAsync();

            return customers;
        }

        #endregion
        public async Task<Truck?> GetTruck(string truckNumber)
        {
            var truck = await _context.Set<Truck>().FirstOrDefaultAsync(x => x.TruckNumber == truckNumber);
            return truck;
        }

        public async Task<Truck> AddTruck(Truck truckEntity)
        {
            var truck = _context.Truck.Add(truckEntity);
            return await Task.FromResult(truck.Entity);
        }

        public async Task<TruckAssignment> AddTruckAssignment(TruckAssignment entity)
        {
            var truckAssignment = _context.TruckAssignment.Add(entity);
            return await Task.FromResult(truckAssignment.Entity);
        }

        public async Task<TruckAssignment?> GetTruckAssignmen(int truckId)
        {
            var truckAssignment = await _context.Set<TruckAssignment>().FirstOrDefaultAsync(x => x.TruckId == truckId);
            return truckAssignment;
        }

        public async Task<List<Fruit>> GetFruits()
        {
            var fruits = await _context.Set<Fruit>().AsNoTracking().ToListAsync();
            return fruits;
        }

        public async Task<Fruit?> GetFruit(int id)
        {
            var fruit = await _context.Set<Fruit>().FirstOrDefaultAsync(x => x.Id == id);
            return fruit;
        }

        public async Task<Fruit> AddFruit(Fruit fruitEntity)
        {
            var fruit = _context.Fruit.Add(fruitEntity);
            return await Task.FromResult(fruit.Entity);
        }

        public async Task<Fruit?> GetFruitByName(string name)
        {
            var fruit = await _context.Set<Fruit>().FirstOrDefaultAsync(x => x.Name == name);
            return fruit;
        }
    }
}
