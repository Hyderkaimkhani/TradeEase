using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Repositories.Interfaces;
using Services.Interfaces;
using System.ComponentModel.Design;
using System.Resources;
using System.Security.Principal;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Services.ServicesImpl
{
    public class SupplyService : ISupplyService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IAdminService adminService;
        private readonly IAccountTransactionService accountTransactionService;

        public SupplyService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              IAdminService adminService,
              IAccountTransactionService accountTransactionService
            )
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.adminService = adminService;
            this.accountTransactionService = accountTransactionService;
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
                        decimal remainingSupplyAmount = supply.TotalPrice;
                        var payableAccount = await unitOfWork.AccountRepository.GetAccountPayable();

                        var unallocatedPayments = await unitOfWork.PaymentRepository.GetUnallocatedPayments(supplier.Id);

                        foreach (var payment in unallocatedPayments)
                        {
                            if (remainingSupplyAmount <= 0) break;

                            var allocatedSum = payment.PaymentAllocations.Sum(a => a.AllocatedAmount);
                            var availableAmount = payment.Amount - allocatedSum;
                            decimal toAllocate = Math.Min(availableAmount, remainingSupplyAmount);

                            await unitOfWork.PaymentRepository.AddPaymentAllocation(new PaymentAllocation
                            {
                                PaymentId = payment.Id,
                                ReferenceType = ReferenceType.Order.ToString(),
                                ReferenceId = supply.Id,
                                AllocatedAmount = toAllocate
                            });

                            remainingSupplyAmount -= toAllocate;
                            // check if amount replicated
                            supply.AmountPaid += toAllocate;
                            supply.PaymentStatus = supply.AmountPaid >= supply.TotalPrice ? PaymentStatus.Paid.ToString() : PaymentStatus.Partial.ToString();
                        }
                    }

                    await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, 0, supply.TotalPrice, OperationType.Supply.ToString());

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        var tranaction = await accountTransactionService.RecordSupplyTransaction(supply.Id, supply.SupplierId, supply.TotalPrice, supply.SupplyDate);
                        if (tranaction.IsError)
                        {
                            // Revert Transaction
                            await DeleteSupply(supply.Id);

                            response.IsError = true;
                            response.Message = tranaction.Message;
                        }
                        else
                        {
                            response.Message = "Supply added successfully";
                            response.Model = autoMapper.Map<SupplyResponseModel>(addedSupply);
                        }
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

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var supply = await unitOfWork.SupplyRepository.GetSupply(requestModel.Id);

                if (supply == null)
                {

                    response.IsError = true;
                    response.Message = "Supply does not exists.";

                }
                else if (!supply.IsActive)
                {
                    response.IsError = true;
                    response.Message = "Supply is inactive and cannot be edited. Contact support if you need to reopen it.";
                }

                if (supply != null)
                {
                    decimal oldTotal = supply.TotalPrice;
                    decimal newTotal = requestModel.Quantity * requestModel.PurchasePrice;
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

                    if (supply.TruckAssignmentId != null && oldTotal != newTotal)
                    {
                        response.IsError = true;
                        response.Message = "Cannot update Supply Quantity or Price, Supply already assigned to Order.";
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

                    supply.Quantity = requestModel.Quantity;
                    supply.PurchasePrice = requestModel.PurchasePrice;
                    supply.TotalPrice = requestModel.Quantity * requestModel.PurchasePrice;
                    supply.TruckId = truck.Id;
                    supply.Notes = requestModel.Notes;
                    supply.SupplyDate = requestModel.SupplyDate;
                    supply.TruckNumber = truck.TruckNumber;

                    //decimal difference = oldTotal - supply.TotalPrice; // 5-3 =2 , 5-10 =-5

                    if (oldTotal != supply.TotalPrice)
                    {
                        await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, oldTotal, supply.TotalPrice, OperationType.Supply.ToString());
                        var transaction = await unitOfWork.AccountTransactionRepository.GetTransaction(OperationType.Supply.ToString(), supply.Id);
                        decimal difference = newTotal - oldTotal;

                        var payablesAccount = await unitOfWork.AccountRepository.GetAccountPayable();

                        // If difference is positive, it means we are increasing the payable amount
                        payablesAccount.CurrentBalance -= difference;

                        if (transaction != null)
                        {
                            transaction.Amount = supply.TotalPrice;
                        }
                        else
                        {
                            response.IsError = true;
                            response.Message = "No transaction found for this supply.";
                            return response;
                        }
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

        public async Task<ResponseModel<List<SupplyResponseModel>>> GetUnassignedSupplies(string? truckNumber)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<List<SupplyResponseModel>>();

                var supplies = await unitOfWork.SupplyRepository.GetUnAssignedSupplies(truckNumber);

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
                else if (supply.TruckAssignmentId != null)
                {
                    response.IsError = true;
                    response.Message = "Cannot delete Supply. Supply already assigned to Order.";
                    return response;
                }
                else
                {
                    if (supply.PaymentStatus == PaymentStatus.Unpaid.ToString())
                    {
                        await adminService.AdjustCustomerBalance(unitOfWork, supply.SupplierId, supply.TotalPrice, 0, OperationType.Supply.ToString());
                        supply.IsActive = false;
                        var transaction = await unitOfWork.AccountTransactionRepository.GetTransaction(OperationType.Supply.ToString(), supply.Id);
                        if (transaction != null)
                        {
                            await accountTransactionService.DeleteTransaction(transaction.Id);
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
                            response.Message = "No transaction found for this supply.";
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
