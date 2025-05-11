using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.ServicesImpl
{
    public class AdminService : IAdminService
    {
        private readonly IMapper autoMapper;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;

        public AdminService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService
            )
        {
            this.autoMapper = autoMapper;
            this.configuration = configuration;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.tokenService = tokenService;
        }

        #region Customer
        public async Task<ResponseModel<CustomerResponseModel>> AddCustomer(CustomerAddModel customerAddModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerResponseModel>();

                var customerExists = await unitOfWork.AdminRepository.GetCustomerByName(customerAddModel.Name);

                if (customerExists != null)
                {
                    response.IsError = true;
                    response.Message = $"{customerExists.EntityType} with same name already exists";
                }
                else
                {
                    var customer = autoMapper.Map<Customer>(customerAddModel);

                    customer.IsActive = true;
                    customer.CreatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.CreatedDate = DateTime.UtcNow;
                    customer.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.UpdatedDate = DateTime.UtcNow;

                    var addedCustomer = await unitOfWork.AdminRepository.AddCustomer(customer);
                    if (addedCustomer != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = $"{customerAddModel.EntityType} added successfuly";
                        response.Model = autoMapper.Map<CustomerResponseModel>(addedCustomer);

                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = $"Unable to add {customerAddModel.EntityType}";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<CustomerResponseModel>> UpdateCustomer(CustomerUpdateModel requestModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerResponseModel>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(requestModel.Id);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = $"Customer does not exists";
                }
                else
                {

                    var customerExists = await unitOfWork.AdminRepository.GetCustomerByName(requestModel.Name);

                    if (customerExists != null && customer.Id != requestModel.Id)
                    {
                        response.IsError = true;
                        response.Message = $"{customerExists.EntityType} with same name already exists";
                    }
                    else
                    {
                        autoMapper.Map(requestModel, customer);

                        customer.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);

                        if (await unitOfWork.SaveChangesAsync())
                        {
                            response.Message = $"{customer.EntityType} updated successfully.";
                            response.Model = autoMapper.Map<CustomerResponseModel>(customer);
                        }
                        else
                        {
                            response.IsError = true;
                            response.Message = $"Unable to update {customer.EntityType}";
                        }
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<List<CustomerResponseModel>>> GetAllCustomers()
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<CustomerResponseModel>>();

                var customers = await unitOfWork.AdminRepository.GetAllCustomers();

                if (customers == null || customers.Count < 1)
                {
                    response.Message = "No Customer found";
                    response.Model = new List<CustomerResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<CustomerResponseModel>>(customers);
                }

                return response;
            }
        }

        public async Task<ResponseModel<List<CustomerResponseModel>>> GetCustomers(bool? isActive, string? entityType)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<CustomerResponseModel>>();

                var customers = await unitOfWork.AdminRepository.GetCustomers(isActive, entityType);

                if (customers == null || customers.Count < 1)
                {
                    response.Message = "No Customer found";
                    response.Model = new List<CustomerResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<CustomerResponseModel>>(customers);
                }

                return response;
            }
        }

        public async Task<List<DropDownModel>> GetCustomersDropDown(string entityType)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var customers = await unitOfWork.AdminRepository.GetCustomersDropDown(entityType);
                return customers;
            }
        }

        public async Task<ResponseModel<CustomerResponseModel>> GetCustomer(int customerId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerResponseModel>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = "Customer does not exists";
                }
                else
                {
                    response.Model = autoMapper.Map<CustomerResponseModel>(customer);
                }

                return response;
            }
        }

        public async Task<ResponseModel<string>> DeleteCustomer(int customerId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<string>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = "Customer does not exists";
                }
                else
                {
                    customer.IsActive = false;
                    customer.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.UpdatedDate = DateTime.UtcNow;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = $"{customer.EntityType} deleted successfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = $"Unable to delete {customer.EntityType}.";
                    }
                }
                return response;
            }
        }

        public async Task AdjustCustomerBalance(IUnitOfWork unitOfWork, int customerId, decimal oldAmount, decimal newAmount, string type)
        {
            var customer = await unitOfWork.AdminRepository.GetCustomer(customerId);

            if (customer == null) throw new Exception("Customer not found");

            // Determine the balance change
            decimal difference = newAmount - oldAmount;

            // For orders/sales, receivable => increase balance
            // For supplies/purchases, payable => decrease balance
            if (type == OperationType.Order.ToString())
            {
                customer.CreditBalance += difference;
            }
            else if (type == OperationType.Supply.ToString())
            {
                customer.CreditBalance -= difference;
            }
        }

        #endregion

        public async Task<ResponseModel<FruitResponseModel>> AddFruit(FruitAddModel requestModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<FruitResponseModel>();

                var fruitExits = await unitOfWork.AdminRepository.GetFruitByName(requestModel.Name);

                if (fruitExits != null)
                {
                    response.IsError = true;
                    response.Message = "Fruit with same name already exists";
                }
                else
                {
                    var fruit = autoMapper.Map<Fruit>(requestModel);

                    fruit.IsActive = true;
                    fruit.CreatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    fruit.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);

                    var addedFruit = await unitOfWork.AdminRepository.AddFruit(fruit);
                    if (addedFruit != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = "Fruit added successfuly";
                        response.Model = autoMapper.Map<FruitResponseModel>(addedFruit);

                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Fruit";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<List<FruitResponseModel>>> GetFruits()
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<FruitResponseModel>>();

                var fruits = await unitOfWork.AdminRepository.GetFruits();

                if (fruits == null || fruits.Count < 1)
                {
                    response.Message = "No Fruit found";
                    response.Model = new List<FruitResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<FruitResponseModel>>(fruits);
                }

                return response;
            }
        }

        public async Task<ResponseModel<FruitResponseModel>> UpdateFruit(FruitAddModel FruitAddModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<FruitResponseModel>();

                var fruit = await unitOfWork.AdminRepository.GetFruit(FruitAddModel.Id);

                if (fruit == null)
                {
                    response.IsError = true;
                    response.Message = "Fruit does not exists";
                }
                else
                {
                    autoMapper.Map(FruitAddModel, fruit);

                    fruit.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Fruit updated successfully.";
                        response.Model = autoMapper.Map<FruitResponseModel>(fruit);
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Fruit";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<FruitResponseModel>> GetFruit(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<FruitResponseModel>();

                var fruit = await unitOfWork.AdminRepository.GetFruit(id);

                if (fruit == null)
                {
                    response.IsError = true;
                    response.Message = "Fruit does not exists";
                }
                else
                {
                    response.Model = autoMapper.Map<FruitResponseModel>(fruit);
                }

                return response;
            }
        }

    }
}
