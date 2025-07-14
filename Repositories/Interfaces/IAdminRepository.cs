using Domain.Entities;
using Domain.Models.ResponseModel;

namespace Repositories.Interfaces
{
    public interface IAdminRepository
    {
        #region Customer
        Task<Customer> AddCustomer(Customer customerEntity);
        Task<Customer?> GetCustomer(int customerId);
        Task<List<Customer>> GetAllCustomers();
        Task<Customer?> GetCustomerByName(string customerName);

        Task<List<Customer>> GetCustomers(bool? isActive, string? entityType);

        Task<List<DropDownModel>> GetCustomersDropDown(string? entityType);
        #endregion

        Task<Truck?> GetTruck(string truckNumber);
        Task<Truck> AddTruck(Truck truckEntity);

        Task<TruckAssignment> AddTruckAssignment(TruckAssignment entity);

        Task<Fruit?> GetFruit(int id);
        Task<Fruit> AddFruit(Fruit fruitntity);
        Task<List<Fruit>> GetFruits();
        Task<Fruit?> GetFruitByName(string name);
    }
}
