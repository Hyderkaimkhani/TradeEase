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
        private readonly IMapper _autoMapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITokenService _tokenService;

        public AdminService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService
            )
        {
            _autoMapper = autoMapper;
            _unitOfWorkFactory = unitOfWorkFactory;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        #region Customer
        public async Task<ResponseModel<CustomerResponseModel>> AddCustomer(CustomerAddModel customerAddModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerResponseModel>();

                var customerExists = await unitOfWork.AdminRepository.GetCustomerByName(customerAddModel.Name);

                if (customerExists != null)
                {
                    response.IsError = true;
                    response.Message = "Customer with same name already exists";
                }
                else
                {
                    var customer = _autoMapper.Map<Customer>(customerAddModel);

                    customer.IsActive = true;
                    customer.CreatedBy = await _tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.CreatedDate = DateTime.Now;
                    customer.UpdatedBy = await _tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.UpdatedDate = DateTime.Now;

                    var addedCustomer = await unitOfWork.AdminRepository.AddCustomer(customer);
                    if (addedCustomer != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = "Customer added successfuly";
                        response.Model = GetCustomer(customer.Id).Result.Model;

                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Customer";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<CustomerResponseModel>> UpdateCustomer(CustomerAddModel customerAddModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerResponseModel>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(customerAddModel.Id);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = "Customer does not exists";
                }
                else
                {
                    _autoMapper.Map(customerAddModel, customer);

                    customer.UpdatedBy = await _tokenService.GetClaimFromToken(ClaimType.Custom_Sub);

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Customer updated successfully.";
                        response.Model = GetCustomer(customer.Id).Result.Model;
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Customer";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<List<CustomerResponseModel>>> GetAllCustomers()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
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
                    response.Model = _autoMapper.Map<List<CustomerResponseModel>>(customers);
                }

                return response;
            }
        }

        public async Task<ResponseModel<List<CustomerResponseModel>>> GetCustomers(bool isActive)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<CustomerResponseModel>>();

                var customers = await unitOfWork.AdminRepository.GetCustomers(isActive);

                if (customers == null || customers.Count < 1)
                {
                    response.Message = "No Customer found";
                    response.Model = new List<CustomerResponseModel>();
                }
                else
                {
                    response.Model = _autoMapper.Map<List<CustomerResponseModel>>(customers);
                }

                return response;
            }
        }

        public async Task<ResponseModel<CustomerResponseModel>> GetCustomer(int customerId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
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
                    response.Model = _autoMapper.Map<CustomerResponseModel>(customer);
                }

                return response;
            }
        }

        public async Task<ResponseModel<string>> DeleteCustomer(int customerId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
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
                    customer.UpdatedBy = await _tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    customer.UpdatedDate = DateTime.Now;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Customer deleted successfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to delete Customer";
                    }
                }
                return response;
            }
        }
        #endregion
    }
}
