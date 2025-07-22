using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Repositories.RepositoriesImpl;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.ServicesImpl
{
    public class BillService : IBillService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public BillService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IAdminService adminService)
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ResponseModel<BillResponseModel>> AddBill(BillAddRequestModel model)
        {
            var response = new ResponseModel<BillResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var customer = await unitOfWork.AdminRepository.GetCustomer(model.EntityId);
                if (customer == null || customer.EntityType != model.EntityType)
                {
                    response.IsError = true;
                    response.Message = $"{model.EntityType} not found or mismatched";
                    return response;
                }

                var fromDate = model.FromDate ?? DateTime.MinValue;
                var toDate = model.ToDate ?? DateTime.UtcNow;

                var referenceType = model.EntityType == EntityType.Customer.ToString() ? OperationType.Order.ToString() : OperationType.Supply.ToString();

                List<Order> orders = new();
                List<Supply> supplies = new();
                List<BillDetail> billDetails = new();
                decimal totalPrice = 0;
                if (referenceType == OperationType.Order.ToString())
                {
                    orders = await unitOfWork.OrderRepository.GetUnbilledOrders(model.EntityId, fromDate, toDate);
                    if (!orders.Any())
                    {
                        response.IsError = true;
                        response.Message = "No unbilled items found in the given date range.";
                        return response;
                    }

                    foreach (var order in orders)
                    {
                        var billDetail = new BillDetail
                        {
                            ReferenceType = referenceType,
                            OrderId = order.Id,
                            ReferenceNumber = order.OrderNumber,
                            Quantity = order.Quantity,
                            UnitPrice = order.SellingPrice,
                            LineTotal = order.TotalSellingPrice,
                        };

                        billDetails.Add(billDetail);
                    }
                    totalPrice = orders.Sum(x => x.TotalSellingPrice);
                }
                else
                {
                    supplies = await unitOfWork.SupplyRepository.GetUnbilledSupplies(model.EntityId, fromDate, toDate);
                    if (!supplies.Any())
                    {
                        response.IsError = true;
                        response.Message = "No unbilled items found in the given date range.";
                        return response;
                    }
                    foreach (var supply in supplies)
                    {
                        var billDetail = new BillDetail
                        {
                            ReferenceType = referenceType,
                            OrderId = supply.Id,
                            ReferenceNumber = supply.SupplyNumber,
                            Quantity = supply.Quantity,
                            UnitPrice = supply.PurchasePrice,
                            LineTotal = supply.TotalPrice,
                        };

                        billDetails.Add(billDetail);
                    }
                    totalPrice = supplies.Sum(x => x.TotalPrice);
                }

                var bill = new Bill
                {
                    BillNumber = $"INV-{Utilities.GenerateRandomNumber()}",
                    EntityId = model.EntityId,
                    EntityType = model.EntityType,
                    EntityName = customer.Name,
                    FromDate = fromDate,
                    ToDate = toDate,
                    DueDate = model.DueDate ?? DateTime.UtcNow,
                    TotalAmount = totalPrice,
                    TotalPaid = 0,      // update when payment is made
                    Balance = totalPrice,
                    Notes = model.Notes,
                };

                bill.BillDetails = billDetails;

                bill = await unitOfWork.BillRepository.AddBill(bill);
                await unitOfWork.SaveChangesAsync();

                response.Model = autoMapper.Map<BillResponseModel>(bill);
                response.Message = "Bill created successfully";
                return response;
            }
        }

        public Task<ResponseModel<BillResponseModel>> UpdateBill(int id, BillAddRequestModel model)
        {
            throw new NotImplementedException();
        }


        public async Task<ResponseModel<BillResponseModel>> GetBill(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var bill = await unitOfWork.BillRepository.GetBill(id);
                if (bill == null)
                    return new ResponseModel<BillResponseModel> { IsError = true, Message = "Bill not found" };

                return new ResponseModel<BillResponseModel>
                {
                    Model = autoMapper.Map<BillResponseModel>(bill)
                };
            }
        }

        public async Task<PaginatedResponseModel<BillResponseModel>> GetBills(FilterModel filterModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<BillResponseModel>();

                var bills = await unitOfWork.BillRepository.GetBills(filterModel);

                if (bills.Model == null || bills.Model.Count < 1)
                {
                    response.Message = "No Bill found";
                    response.Model = new List<BillResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<BillResponseModel>>(bills.Model);
                    response.TotalCount = bills.TotalCount;
                }
                return response;
            }
        }

        public async Task<ResponseModel<BillResponseModel>> UpdateBill(BillUpdateRequestModel model)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var reponse = new ResponseModel<BillResponseModel>();
                var bill = await unitOfWork.BillRepository.GetBill(model.Id);
                if (bill == null)
                    return new ResponseModel<BillResponseModel> { IsError = true, Message = "Bill not found" };

                bill.DueDate = model.DueDate ?? bill.DueDate;
                bill.Notes = model.Notes ?? bill.Notes;

                await unitOfWork.SaveChangesAsync();

                reponse.Model = autoMapper.Map<BillResponseModel>(bill);
                reponse.Message = "Bill updated successfully";

                return reponse;
            }
        }

        public async Task<ResponseModel<bool>> DeleteBill(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var bill = await unitOfWork.BillRepository.GetBill(id);
                if (bill == null)
                    return new ResponseModel<bool> { IsError = true, Message = "Bill not found", Model = false };

                bill.IsActive = false; // Soft delete

                await unitOfWork.SaveChangesAsync();
                return new ResponseModel<bool>
                {
                    Message = "Bill deleted successfully",
                    Model = true
                };
            }
        }

        public async Task UpdateBillPaymentStatus(List<int> billIds)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                foreach (var billId in billIds)
                {
                    var bill = await unitOfWork.BillRepository.GetBill(billId);

                    if (bill == null) throw new Exception("Bill not found");

                    decimal totalAmount = 0;
                    decimal totalPaid = 0;

                    foreach (var detail in bill.BillDetails)
                    {
                        if (detail.ReferenceType == "Order" && detail.OrderId.HasValue)
                        {
                            var order = await unitOfWork.OrderRepository.GetOrder(detail.OrderId.Value);
                            if (order != null)
                            {
                                totalAmount += order.TotalSellingPrice;
                                totalPaid += order.AmountReceived;
                            }
                        }
                        else if (detail.ReferenceType == "Supply" && detail.SupplyId.HasValue)
                        {
                            var supply = await unitOfWork.SupplyRepository.GetSupply(detail.SupplyId.Value);
                            if (supply != null)
                            {
                                totalAmount += supply.TotalPrice;
                                totalPaid += supply.AmountPaid;
                            }
                        }
                    }

                    bill.TotalAmount = totalAmount;
                    bill.TotalPaid = totalPaid;
                    bill.Balance = totalAmount - totalPaid;

                    if (totalPaid == 0)
                        bill.PaymentStatus = PaymentStatus.Unpaid.ToString();
                    else if (totalPaid >= totalAmount)
                        bill.PaymentStatus = PaymentStatus.Paid.ToString();
                    else
                        bill.PaymentStatus = PaymentStatus.Partial.ToString();
                    await unitOfWork.SaveChangesAsync();
                }
            }
        }

    }
}
