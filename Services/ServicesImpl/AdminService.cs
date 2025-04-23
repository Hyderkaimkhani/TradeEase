using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

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

        #region Client
        public async Task<ResponseModel<ClientModel>> AddClient(ClientModel clientModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ClientModel>();

                var clientExists = await unitOfWork.AdminRepository.GetClientByName(clientModel.Name);

                if (clientExists != null)
                {
                    response.IsError = true;
                    response.Message = "Client already exists";
                }
                else
                {
                    var client = _autoMapper.Map<Client>(clientModel);

                    client.IsActive = true;
                    client.CreatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    client.CreatedDate = DateTime.Now;
                    client.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    client.UpdatedDate = DateTime.Now;

                    var addedClient = await unitOfWork.AdminRepository.AddClient(client);
                    if (addedClient != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = "Client added successfuly";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Client";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<ClientModel>> UpdateClient(ClientModel clientModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ClientModel>();

                var client = await unitOfWork.AdminRepository.GetClient(clientModel.Id);

                if (client == null)
                {
                    response.IsError = true;
                    response.Message = "Client does not exists";
                }
                else
                {
                    clientModel.CreatedDate = client.CreatedDate;
                    clientModel.CreatedBy = client.CreatedBy;
                    clientModel.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    clientModel.UpdatedDate = DateTime.Now;

                    _autoMapper.Map(clientModel, client);

                    //   client.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    //   client.UpdatedDate = DateTime.Now;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Client updated successfully.";
                        response.Model = clientModel;
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Client";
                    }
                }
                return response;
            }
        }

        public async Task<List<ClientModel>> GetAllClients()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new List<ClientModel>();

                var clients = await unitOfWork.AdminRepository.GetAllClients();

                if (clients == null)
                {
                    response = null;
                }
                else
                {
                    response = _autoMapper.Map<List<ClientModel>>(clients);
                }

                return response;
            }
        }

        public async Task<ResponseModel<ClientModel>> GetClient(int clientId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ClientModel>();

                var client = await unitOfWork.AdminRepository.GetClient(clientId);

                if (client == null)
                {
                    response.IsError = true;
                    response.Message = "Client does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<ClientModel>(client);
                }

                return response;
            }
        }

        public async Task<ResponseModel<string>> DeleteClient(int clientId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<string>();

                var client = await unitOfWork.AdminRepository.GetClient(clientId);

                if (client == null)
                {
                    response.IsError = true;
                    response.Message = "Client does not exists";
                }
                else
                {
                    client.IsActive = false;
                    client.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    client.UpdatedDate = DateTime.Now;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Client deleted successfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to delete Client";
                    }
                }
                return response;
            }
        }
        #endregion

        #region Consumer
        public async Task<ResponseModel<ConsumerModel>> AddConsumer(ConsumerModel consumerModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ConsumerModel>();

                var consumerExists = await unitOfWork.AdminRepository.GetConsumerByName(consumerModel.FirstName);

                if (consumerExists != null)
                {
                    response.IsError = true;
                    response.Message = "Consumer already exists";
                }
                else
                {
                    var consumer = _autoMapper.Map<Consumer>(consumerModel);

                    consumer.IsActive = true;
                    consumer.CreatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    consumer.CreatedDate = DateTime.Now;
                    consumer.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    consumer.UpdatedDate = DateTime.Now;

                    var addedConsumer = await unitOfWork.AdminRepository.AddConsumer(consumer);
                    if (addedConsumer != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = "Consumer added successfuly";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Consumer";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<ConsumerModel>> GetConsumer(int consumerId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ConsumerModel>();

                var consumer = await unitOfWork.AdminRepository.GetConsumer(consumerId);

                if (consumer == null)
                {
                    response.IsError = true;
                    response.Message = "Consumer does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<ConsumerModel>(consumer);
                }

                return response;
            }
        }

        public async Task<List<ConsumerModel>> GetAllConsumers()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new List<ConsumerModel>();

                var consumers = await unitOfWork.AdminRepository.GetAllConsumers();

                if (consumers == null)
                {
                    response = null;
                }
                else
                {
                    response = _autoMapper.Map<List<ConsumerModel>>(consumers);
                }

                return response;
            }
        }

        public async Task<ResponseModel<ConsumerModel>> UpdateConsumer(ConsumerModel consumertModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<ConsumerModel>();

                var consumer = await unitOfWork.AdminRepository.GetConsumerForUpdate(consumertModel.Id);

                if (consumer == null)
                {
                    response.IsError = true;
                    response.Message = "Consumer does not exists";
                }
                else
                {
                    consumertModel.CreatedDate = consumer.CreatedDate;
                    consumertModel.CreatedBy = consumer.CreatedBy;
                    consumertModel.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    consumertModel.UpdatedDate = DateTime.Now;

                    _autoMapper.Map(consumertModel, consumer);
                   
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Consumer updated successfully.";
                        response.Model = consumertModel;
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Consumer";
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<string>> DeleteConsumer(int consumerId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<string>();

                var consumer = await unitOfWork.AdminRepository.GetConsumer(consumerId);

                if (consumer == null)
                {
                    response.IsError = true;
                    response.Message = "Consumer does not exists";
                }
                else
                {
                    consumer.IsActive = false;
                    consumer.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    consumer.UpdatedDate = DateTime.Now;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Consumer deleted successfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to delete Consumer";
                    }
                }
                return response;
            }
        }
        #endregion

        #region Client
        public async Task<ResponseModel<CustomerModel>> AddCustomer(CustomerModel customerModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerModel>();

                var customerExists = await unitOfWork.AdminRepository.GetCustomerByName(customerModel.Name);

                if (customerExists != null)
                {
                    response.IsError = true;
                    response.Message = "Customer with same name already exists";
                }
                else
                {
                    var customer = _autoMapper.Map<Customer>(customerModel);

                    customer.IsActive = true;
                    customer.CreatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    customer.CreatedDate = DateTime.Now;
                    customer.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    customer.UpdatedDate = DateTime.Now;

                    var addedCustomer = await unitOfWork.AdminRepository.AddCustomer(customer);
                    if (addedCustomer != null)
                    {
                        await unitOfWork.SaveChangesAsync();
                        response.Message = "Customer added successfuly";
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

        public async Task<ResponseModel<CustomerModel>> UpdateCustomer(CustomerModel customerModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerModel>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(customerModel.Id);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = "Customer does not exists";
                }
                else
                {
                    customerModel.CreatedDate = customer.CreatedDate;
                    customerModel.CreatedBy = customer.CreatedBy;
                    customerModel.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    customerModel.UpdatedDate = DateTime.Now;

                    _autoMapper.Map(customerModel, customer);

                    //   Customer.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    //   Customer.UpdatedDate = DateTime.Now;

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Customer updated successfully.";
                        response.Model = customerModel;
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

        public async Task<List<CustomerModel>> GetAllCustomers()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new List<CustomerModel>();

                var customers = await unitOfWork.AdminRepository.GetAllCustomers();

                if (customers == null)
                {
                    response = null;
                }
                else
                {
                    response = _autoMapper.Map<List<CustomerModel>>(customers);
                }

                return response;
            }
        }

        public async Task<ResponseModel<CustomerModel>> GetCustomer(int customerId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CustomerModel>();

                var customer = await unitOfWork.AdminRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = "Customer does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<CustomerModel>(customer);
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
                    customer.UpdatedBy = await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
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
