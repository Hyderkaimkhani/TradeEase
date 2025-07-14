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
    public class PaymentService : IPaymentService
    {
        private readonly IMapper autoMapper;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;

        public PaymentService(IUnitOfWorkFactory unitOfWorkFactory,
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

        public async Task<ResponseModel<PaymentResponseModel>> AddPayment(PaymentAddModel requestModel)
        {
            var response = new ResponseModel<PaymentResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var customer = await unitOfWork.AdminRepository.GetCustomer(requestModel.EntityId);
                if (customer == null || customer.EntityType != requestModel.EntityType)
                {
                    response.Message = $"{requestModel.EntityType} not found or mismatched";
                    response.IsError = true;
                    return response;
                }

                var payment = new Payment
                {
                    EntityType = requestModel.EntityType,
                    EntityId = requestModel.EntityId,
                    Amount = requestModel.Amount,
                    PaymentDate = requestModel.PaymentDate,
                    PaymentMethod = requestModel.PaymentMethod,
                    Notes = requestModel.Notes,
                };

                payment = await unitOfWork.PaymentRepository.AddPayment(payment);

                await unitOfWork.SaveChangesAsync();
                decimal remainingAmount = requestModel.Amount;

                if (requestModel.EntityType == EntityType.Customer.ToString())
                {
                    if (customer.CreditBalance < 0)
                    {
                        remainingAmount -= customer.CreditBalance; // If customer has credit balance, add it to remaining amount to allocate
                    }
                    var orders = await unitOfWork.OrderRepository.GetUnpaidOrders(requestModel.EntityId);

                    foreach (var order in orders)
                    {
                        var remainingDue = order.TotalSellingPrice - order.AmountReceived;

                        if (remainingAmount <= 0) break;

                        var toAllocate = Math.Min(remainingAmount, remainingDue);

                        var paymentAllocation = new PaymentAllocation
                        {
                            PaymentId = payment.Id,
                            ReferenceType = OperationType.Order.ToString(),
                            ReferenceId = order.Id,
                            AllocatedAmount = toAllocate,
                        };

                        await unitOfWork.PaymentRepository.AddPaymentAllocation(paymentAllocation);
                        order.AmountReceived += toAllocate;
                        order.PaymentStatus = order.AmountReceived >= order.TotalSellingPrice ? PaymentStatus.Paid.ToString() : PaymentStatus.Partial.ToString();

                        remainingAmount -= toAllocate;
                    }

                    customer.CreditBalance -= requestModel.Amount;
                }
                else if (requestModel.EntityType == EntityType.Supplier.ToString())
                {
                    if (customer.CreditBalance > 0)
                    {
                        remainingAmount += customer.CreditBalance; // If customer has credit balance, add it to remaining amount to allocate
                    }
                    var supplies = await unitOfWork.SupplyRepository.GetUnpaidSupplies(requestModel.EntityId);

                    foreach (var supply in supplies)
                    {
                        var remainingDue = supply.TotalPrice - supply.AmountPaid;

                        if (remainingAmount <= 0) break;

                        var toAllocate = Math.Min(remainingAmount, remainingDue);

                        var paymentAllocation = new PaymentAllocation
                        {
                            PaymentId = payment.Id,
                            ReferenceType = OperationType.Supply.ToString(),
                            ReferenceId = supply.Id,
                            AllocatedAmount = toAllocate,
                        };
                        await unitOfWork.PaymentRepository.AddPaymentAllocation(paymentAllocation);

                        supply.AmountPaid += toAllocate;
                        supply.PaymentStatus = supply.AmountPaid >= supply.TotalPrice ? PaymentStatus.Paid.ToString() : PaymentStatus.Partial.ToString();

                        remainingAmount -= toAllocate;
                    }
                    customer.CreditBalance += requestModel.Amount;  // 
                }

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Message = "Payment added successfuly";

                    response.Model = autoMapper.Map<PaymentResponseModel>(payment);

                }
                return response;
            }
        }

        public async Task<ResponseModel<PaymentResponseModel>> UpdatePayment(PaymentUpdateModel requestModel)
        {
            var response = new ResponseModel<PaymentResponseModel>();
            using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            var payment = await unitOfWork.PaymentRepository.GetPayment(requestModel.Id);
            if (payment == null || !payment.IsActive)
            {
                response.IsError = true;
                response.Message = "Payment not found";
                return response;
            }

            if (payment.Amount != requestModel.Amount)
            {

                var customer = await unitOfWork.AdminRepository.GetCustomer(payment.EntityId);
                if (customer == null || customer.EntityType != payment.EntityType)
                {
                    response.IsError = true;
                    response.Message = $"{payment.EntityType} not found or mismatched";
                    return response;
                }

                decimal oldAmount = payment.Amount;
                decimal originalAllocated = payment.PaymentAllocations.Where(x => x.IsActive == true).Sum(x => x.AllocatedAmount);

                // Reverse Allocations
                foreach (var alloc in payment.PaymentAllocations)
                {
                    if (alloc.ReferenceType == OperationType.Order.ToString())
                    {
                        var order = await unitOfWork.OrderRepository.GetOrder(alloc.ReferenceId);
                        if (order != null)
                        {
                            order.AmountReceived -= alloc.AllocatedAmount;
                            order.PaymentStatus = order.AmountReceived == 0 ? PaymentStatus.Unpaid.ToString()
                                                 : order.AmountReceived < order.TotalSellingPrice ? PaymentStatus.Partial.ToString()
                                                 : PaymentStatus.Paid.ToString();
                        }
                    }
                    else if (alloc.ReferenceType == OperationType.Supply.ToString())
                    {
                        var supply = await unitOfWork.SupplyRepository.GetSupply(alloc.ReferenceId);
                        if (supply != null)
                        {
                            supply.AmountPaid -= alloc.AllocatedAmount;
                            supply.PaymentStatus = supply.AmountPaid == 0 ? PaymentStatus.Unpaid.ToString()
                                                 : supply.AmountPaid < supply.TotalPrice ? PaymentStatus.Partial.ToString()
                                                 : PaymentStatus.Paid.ToString();
                        }
                    }

                    alloc.IsActive = false;
                    //alloc.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                }

                // Update Payment info
                payment.Amount = requestModel.Amount;
                payment.PaymentDate = requestModel.PaymentDate;
                payment.PaymentMethod = requestModel.PaymentMethod;
                payment.Notes = requestModel.Notes;
                //payment.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                //payment.UpdatedDate = DateTime.Now;

                await unitOfWork.SaveChangesAsync();        // save, to get latest status of supplies. Reverse Allocations not reflect in db otherwise

                decimal remainingAmount = requestModel.Amount;

                if (payment.EntityType == EntityType.Customer.ToString())
                {
                    if (customer.CreditBalance < 0)
                    {
                        remainingAmount -= customer.CreditBalance; // If customer has credit balance, add it to remaining amount to allocate
                    }
                    var orders = await unitOfWork.OrderRepository.GetUnpaidOrders(payment.EntityId);

                    foreach (var order in orders)
                    {
                        var due = order.TotalSellingPrice - order.AmountReceived;
                        if (remainingAmount <= 0) break;

                        var toAllocate = Math.Min(remainingAmount, due);

                        await unitOfWork.PaymentRepository.AddPaymentAllocation(new PaymentAllocation
                        {
                            PaymentId = payment.Id,
                            ReferenceType = OperationType.Order.ToString(),
                            ReferenceId = order.Id,
                            AllocatedAmount = toAllocate
                        });

                        order.AmountReceived += toAllocate;
                        order.PaymentStatus = order.AmountReceived >= order.TotalSellingPrice
                                              ? PaymentStatus.Paid.ToString()
                                              : PaymentStatus.Partial.ToString();

                        remainingAmount -= toAllocate;
                    }

                    //customer.CreditBalance += (requestModel.Amount - (oldAmount - originalAllocated));
                    decimal difference = requestModel.Amount - oldAmount;
                    customer.CreditBalance -= difference;
                }
                else if (payment.EntityType == EntityType.Supplier.ToString())
                {
                    if (customer.CreditBalance > 0)
                    {
                        remainingAmount += customer.CreditBalance; // If customer has credit balance, add it to remaining amount to allocate
                    }

                    var supplies = await unitOfWork.SupplyRepository.GetUnpaidSupplies(payment.EntityId);

                    foreach (var supply in supplies)
                    {
                        var due = supply.TotalPrice - supply.AmountPaid;
                        if (remainingAmount <= 0) break;

                        var toAllocate = Math.Min(remainingAmount, due);

                        await unitOfWork.PaymentRepository.AddPaymentAllocation(new PaymentAllocation
                        {
                            PaymentId = payment.Id,
                            ReferenceType = OperationType.Supply.ToString(),
                            ReferenceId = supply.Id,
                            AllocatedAmount = toAllocate
                        });

                        supply.AmountPaid += toAllocate;
                        supply.PaymentStatus = supply.AmountPaid >= supply.TotalPrice
                                               ? PaymentStatus.Paid.ToString()
                                               : PaymentStatus.Partial.ToString();

                        remainingAmount -= toAllocate;
                    }
                    decimal difference = requestModel.Amount - oldAmount;
                    customer.CreditBalance += difference;
                }
            }
            else
            {
                payment.PaymentDate = requestModel.PaymentDate;
                payment.PaymentMethod = requestModel.PaymentMethod;
                payment.Notes = requestModel.Notes;
                //payment.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
            }

            if (await unitOfWork.SaveChangesAsync())
            {
                response.Message = "Payment updated successfully";
                response.Model = autoMapper.Map<PaymentResponseModel>(payment);
            }

            return response;
        }

        public async Task<ResponseModel<PaymentResponseModel>> GetPayment(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<PaymentResponseModel>();

                var payment = await unitOfWork.PaymentRepository.GetPayment(id);

                if (payment == null)
                {
                    response.IsError = true;
                    response.Message = "Payment does not exists";
                }
                else
                {
                    var paymentModel = autoMapper.Map<PaymentResponseModel>(payment);

                    foreach (var paymentAllocation in paymentModel.PaymentAllocations)
                    {
                        if (paymentAllocation.ReferenceType == OperationType.Order.ToString())
                        {
                            var order = await unitOfWork.OrderRepository.GetOrder(paymentAllocation.ReferenceId);
                            if (order != null)
                            {
                                paymentAllocation.Number = order.OrderNumber;
                                paymentAllocation.TruckNumber = order.TruckNumber;
                                paymentAllocation.Price = order.SellingPrice;
                                paymentAllocation.Quantity = order.Quantity;
                                paymentAllocation.OperationDate = order.OrderDate;
                                paymentAllocation.TotalPrice = order.TotalSellingPrice;
                                paymentAllocation.PaymentStatus = order.PaymentStatus;

                            }
                        }
                        else if (paymentAllocation.ReferenceType == OperationType.Supply.ToString())
                        {
                            var supply = await unitOfWork.SupplyRepository.GetSupply(paymentAllocation.ReferenceId);
                            if (supply != null)
                            {
                                paymentAllocation.Number = supply.SupplyNumber;
                                paymentAllocation.TruckNumber = supply.TruckNumber;
                                paymentAllocation.Price = supply.PurchasePrice;
                                paymentAllocation.Quantity = supply.Quantity;
                                paymentAllocation.OperationDate = supply.SupplyDate;
                                paymentAllocation.TotalPrice = supply.TotalPrice;
                                paymentAllocation.PaymentStatus = supply.PaymentStatus;
                            }
                        }
                    }
                    response.Model = paymentModel;
                }

                return response;
            }
        }

        public async Task<PaginatedResponseModel<PaymentResponseModel>> GetPayments(int page, int pageSize, int? customerId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<PaymentResponseModel>();

                var payments = await unitOfWork.PaymentRepository.GetPayments(page, pageSize, customerId, null);

                if (payments.Model == null || payments.Model.Count < 1)
                {
                    response.Message = "No Payment found";
                    response.Model = new List<PaymentResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<PaymentResponseModel>>(payments.Model);
                    response.TotalCount = payments.TotalCount;
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> DeletePayment(int paymentId)
        {
            var response = new ResponseModel<bool>();
            using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            var payment = await unitOfWork.PaymentRepository.GetPayment(paymentId);
            if (payment == null || !payment.IsActive)
            {
                response.IsError = true;
                response.Message = "Payment already deleted.";
                return response;
            }

            var customer = await unitOfWork.AdminRepository.GetCustomer(payment.EntityId);
            if (customer == null || customer.EntityType != payment.EntityType)
            {
                response.IsError = true;
                response.Message = $"{payment.EntityType} not found or mismatched";
                return response;
            }

            decimal allocatedTotal = 0;

            foreach (var alloc in payment.PaymentAllocations)
            {
                allocatedTotal += alloc.AllocatedAmount;

                if (alloc.ReferenceType == OperationType.Order.ToString())
                {
                    var order = await unitOfWork.OrderRepository.GetOrder(alloc.ReferenceId);
                    if (order != null)
                    {
                        order.AmountReceived -= alloc.AllocatedAmount;
                        order.PaymentStatus = order.AmountReceived == 0 ? PaymentStatus.Unpaid.ToString()
                                             : order.AmountReceived < order.TotalSellingPrice ? PaymentStatus.Partial.ToString()
                                             : PaymentStatus.Paid.ToString();
                    }
                }
                else if (alloc.ReferenceType == OperationType.Supply.ToString())
                {
                    var supply = await unitOfWork.SupplyRepository.GetSupply(alloc.ReferenceId);
                    if (supply != null)
                    {
                        supply.AmountPaid -= alloc.AllocatedAmount;
                        supply.PaymentStatus = supply.AmountPaid == 0 ? PaymentStatus.Unpaid.ToString()
                                             : supply.AmountPaid < supply.TotalPrice ? PaymentStatus.Partial.ToString()
                                             : PaymentStatus.Paid.ToString();
                    }
                }

                alloc.IsActive = false;
            }

            // Reverse customer balance
            if (payment.EntityType == EntityType.Customer.ToString())
            {
                customer.CreditBalance += payment.Amount;
            }
            else if (payment.EntityType == EntityType.Supplier.ToString())
            {
                customer.CreditBalance -= payment.Amount;
            }

            payment.IsActive = false;

            if (await unitOfWork.SaveChangesAsync())
            {
                response.Message = "Payment deleted successfully";
                response.Model = true;
            }

            return response;
        }

    }
}
