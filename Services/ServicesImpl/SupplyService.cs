using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Services.ServicesImpl
{
    public class SupplyService : ISupplyService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;
        private readonly IAdminService adminService;

        public SupplyService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService,
              IAdminService adminService
            )
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.tokenService = tokenService;
            this.adminService = adminService;
        }

        public async Task<ResponseModel<SupplyResponseModel>> AddSupply(SupplyAddModel requestModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<SupplyResponseModel>();

                var supplier = await unitOfWork.AdminRepository.GetCustomer(requestModel.SupplierId);
                if (supplier == null)
                {
                    response.Message = $"Supplier not found or mismatched";
                    response.IsError = true;
                    return response;
                }

                // Get Truck if not exists add
                Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);

                if (truck == null)
                {
                    var truckEntity = new Truck
                    {
                        TruckNumber = requestModel.TruckNumber,
                        IsActive = true
                    };
                    truck = await unitOfWork.AdminRepository.AddTruck(truckEntity);
                    await unitOfWork.SaveChangesAsync();
                }

                var supply = autoMapper.Map<Supply>(requestModel);

                supply.TotalPrice = requestModel.Quantity * requestModel.PurchasePrice;
                supply.SupplyNumber = "S-" + Utilities.GenerateRandomNumber();
                supply.TruckId = truck.Id;
                supply.IsActive = true;

                var addedSupply = await unitOfWork.SupplyRepository.AddSupply(supply);
                if (addedSupply != null)
                {
                    if (supplier.CreditBalance > 0) // If supplier has paid in advance, try to allocate from it
                    {
                        var allocatable = Math.Min(supplier.CreditBalance, supply.TotalPrice);
                        if (allocatable > 0)
                        {
                            var payment = new Payment
                            {
                                EntityId = supply.SupplierId,
                                Amount = allocatable,
                                PaymentDate = DateTime.Now,
                                PaymentMethod = "CreditBalance Auto",
                                Notes = "Auto allocation from Credit Balance",
                            };

                            await unitOfWork.PaymentRepository.AddPayment(payment);
                            await unitOfWork.SaveChangesAsync();

                            var allocation = new PaymentAllocation
                            {
                                PaymentId = payment.Id,
                                ReferenceType = OperationType.Supply.ToString(),
                                ReferenceId = supply.Id,
                                AllocatedAmount = allocatable
                            };

                            await unitOfWork.PaymentRepository.AddPaymentAllocation(allocation);

                            supply.AmountPaid += allocatable;
                            supply.PaymentStatus = supply.AmountPaid >= supply.TotalPrice ? PaymentStatus.Paid.ToString() : PaymentStatus.Partial.ToString();
                            //supplier.CreditBalance -= allocatable; // will adjust in AdjustCustomerBalance
                        }
                    }

                    await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, 0, supply.TotalPrice, OperationType.Supply.ToString());

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Supply added successfully";
                        response.Model = autoMapper.Map<SupplyResponseModel>(addedSupply);
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Supply";
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Unable to add Supply";
                }

                return response;
            }
        }

        public async Task<ResponseModel<SupplyResponseModel>> UpdateSupply(SupplyUpdateModel requestModel)
        {
            var response = new ResponseModel<SupplyResponseModel>();
            // Check if Bill is created against this Order, if yes then restrict to update Price.
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var supply = await unitOfWork.SupplyRepository.GetSupply(requestModel.Id);

                if (supply != null)
                {
                    //Restrict to change price and quantity if payment made against this supply.
                    if (supply.PaymentStatus != PaymentStatus.Unpaid.ToString() && (supply.Quantity != requestModel.Quantity || supply.PurchasePrice != requestModel.PurchasePrice))
                    {
                        response.IsError = true;
                        response.Message = "Cannot update Supply Quantity or Price, payment already made against this supply.";
                        return response;
                    }
                    //Don't allow to update truck if Supply has allocated TruckAssignmentId
                    if (supply.Truck.TruckNumber != requestModel.TruckNumber && supply.TruckAssignmentId != null)
                    {
                        response.IsError = true;
                        response.Message = "Cannot update Supply TruckNumber, Truck already assigned";
                        return response;
                    }

                    Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);
                    
                    if (truck == null)
                    {
                        var truckEntity = new Truck
                        {
                            TruckNumber = requestModel.TruckNumber,
                            IsActive = true
                        };
                        truck = await unitOfWork.AdminRepository.AddTruck(truckEntity);
                        await unitOfWork.SaveChangesAsync();
                    }

                    decimal oldTotal = supply.TotalPrice;

                    supply.Quantity = requestModel.Quantity;
                    supply.PurchasePrice = requestModel.PurchasePrice;
                    supply.TotalPrice = requestModel.Quantity * requestModel.PurchasePrice;
                    supply.TruckId = truck.Id;
                    supply.Notes = requestModel.Notes;
                    supply.SupplyDate = requestModel.SupplyDate;

                    //decimal difference = oldTotal - supply.TotalPrice; // 5-3 =2 , 5-10 =-5

                    if (oldTotal != supply.TotalPrice)
                    {
                        await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, oldTotal, supply.TotalPrice, OperationType.Supply.ToString());
                    }

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Supply updated successfully.";
                        response.Model = autoMapper.Map<SupplyResponseModel>(supply);
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Supply.";
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Supply does not exists.";
                }
            }
            return response;
        }

        public async Task<ResponseModel<List<SupplyResponseModel>>> GetAllSupplies()
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<SupplyResponseModel>>();

                var supplies = await unitOfWork.SupplyRepository.GetSupplies(true);

                if (supplies == null || supplies.Count < 1)
                {
                    response.Message = "No Supply found";
                    response.Model = new List<SupplyResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<SupplyResponseModel>>(supplies);
                }

                return response;
            }
        }

        public async Task<ResponseModel<SupplyResponseModel>> GetSupply(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<SupplyResponseModel>();

                var supply = await unitOfWork.SupplyRepository.GetSupply(id);

                if (supply == null)
                {
                    response.IsError = true;
                    response.Message = "Supply does not exists";
                }
                else
                {
                    response.Model = autoMapper.Map<SupplyResponseModel>(supply);
                }

                return response;
            }
        }

        public async Task<PaginatedResponseModel<SupplyResponseModel>> GetSupplies(int page, int pageSize, int? fruitId, int? supplierId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<SupplyResponseModel>();

                var supplies = await unitOfWork.SupplyRepository.GetSupplies(page, pageSize, fruitId, supplierId);

                if (supplies.Model == null || supplies.Model.Count < 1)
                {
                    response.Message = "No Supply found";
                    response.Model = new List<SupplyResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<SupplyResponseModel>>(supplies.Model);
                    response.TotalCount = supplies.TotalCount;
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> DeleteSupply(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<bool>();
                var supply = await unitOfWork.SupplyRepository.GetSupply(id);


                if (supply == null || !supply.IsActive)
                {
                    response.IsError = true;
                    response.Message = "Supply does not exists";
                }
                else
                {
                    if (supply.PaymentStatus == PaymentStatus.Unpaid.ToString())
                    {
                        await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, supply.TotalPrice, 0, OperationType.Supply.ToString());
                        supply.IsActive = false;
                        if (await unitOfWork.SaveChangesAsync())
                        {
                            response.Model = true;
                            response.Message = "Supply deleted successfully.";
                        }
                        else
                        {
                            response.IsError = true;
                            response.Message = "Unable to delete Supply";
                            response.Model = false;
                        }
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Cannot delete Supply, payment already made against this supply.";
                        response.Model = false;
                    }
                }
                return response;
            }
        }

    }
}
