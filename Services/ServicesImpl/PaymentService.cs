using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.ServicesImpl
{
    public class PaymentService : IPaymentService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IBillService billService;
        private readonly IAccountTransactionService accountTransactionService;

        public PaymentService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IBillService billService, IAccountTransactionService accountTransactionService
            )
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.billService = billService;
            this.accountTransactionService = accountTransactionService;
        }

        public async Task<ResponseModel<PaymentResponseModel>> AddPayment(PaymentAddModel requestModel)
        {
            var response = new ResponseModel<PaymentResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var customer = await unitOfWork.AdminRepository.GetCustomer(requestModel.EntityId);
                if (customer == null)
                {
                    response.Message = $"Customer not found or mismatched";
                    response.IsError = true;
                    return response;
                }

                var payment = new Payment
                {
                    EntityId = requestModel.EntityId,
                    AccountId = requestModel.AccountId,
                    TransactionFlow = requestModel.TransactionFlow,
                    Amount = requestModel.Amount,
                    PaymentDate = requestModel.PaymentDate,
                    PaymentMethod = requestModel.PaymentMethod,
                    Notes = requestModel.Notes,
                };

                payment = await unitOfWork.PaymentRepository.AddPayment(payment);

                await unitOfWork.SaveChangesAsync();

                decimal remainingAmount = requestModel.Amount; //10000


                var transaction = new AccountTransactionAddModel
                {
                    AccountId = requestModel.AccountId,
                    TransactionType = TransactionType.Payment.ToString(),
                    TransactionDirection = requestModel.TransactionFlow == TransactionFlow.Received.ToString() ? TransactionDirection.Debit.ToString() : TransactionDirection.Credit.ToString(),
                    Amount = requestModel.Amount,
                    TransactionDate = requestModel.PaymentDate,
                    PaymentMethod = requestModel.PaymentMethod,
                    EntityId = requestModel.EntityId,
                    ReferenceType = ReferenceType.Payment.ToString(),
                    ReferenceId = payment.Id, // Set ReferenceId to Payment Id
                    Notes = requestModel.Notes
                };

                var accountTransaction = await accountTransactionService.AddTransaction(transaction);
                if (accountTransaction.IsError)
                {
                    response.IsError = true;
                    response.Message = accountTransaction.Message;
                    return response;
                }


                if (requestModel.TransactionFlow == TransactionFlow.Received.ToString())
                {
                    //requestModel.TransactionDirection = TransactionDirection.Debit.ToString();
                    // Record for Internal Account Transaction
                    //var accountTransaction = await accountTransactionService.RecordPaymentTransaction(requestModel);
                    //if (accountTransaction.IsError)
                    //{
                    //    response.IsError = true;
                    //    response.Message = accountTransaction.Message;
                    //    return response;
                    //}

                    // Record for Account Receivables
                    var receivableAccount = await unitOfWork.AccountRepository.GetAccountReceivable();
                    transaction.TransactionDirection = TransactionDirection.Credit.ToString();
                    transaction.AccountId = receivableAccount!.Id; // Set AccountId to Receivables account

                    var receivableTransaction = await accountTransactionService.AddTransaction(transaction);
                    if (receivableTransaction.IsError)
                    {
                        response.IsError = true;
                        response.Message = receivableTransaction.Message;
                        return response;
                    }

                    if (customer.CreditBalance < 0)
                    {
                        // You owe them (advance payment given) — so payment increases allocatable amount
                        remainingAmount += Math.Abs(customer.CreditBalance);
                    }

                    var unpaidOrders = await unitOfWork.OrderRepository.GetUnpaidOrders(requestModel.EntityId);

                    foreach (var order in unpaidOrders)
                    {
                        if (remainingAmount <= 0) break;

                        var remainingDue = order.TotalSellingPrice - order.AmountReceived;
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
                else if (requestModel.TransactionFlow == TransactionFlow.Paid.ToString())
                {
                    //requestModel.TransactionDirection = TransactionDirection.Credit.ToString();
                    //// Record for Internal Account Transaction
                    //var accountTransaction = await accountTransactionService.RecordPaymentTransaction(requestModel);
                    //if (accountTransaction.IsError)
                    //{
                    //    response.IsError = true;
                    //    response.Message = accountTransaction.Message;
                    //    return response;
                    //}

                    // Record for Account Payable
                    var payableAccount = await unitOfWork.AccountRepository.GetAccountPayable();
                    transaction.TransactionDirection = TransactionDirection.Debit.ToString();
                    transaction.AccountId = payableAccount!.Id; // Set AccountId to Payable account
                    var payableTransaction = await accountTransactionService.AddTransaction(transaction);
                    if (payableTransaction.IsError)
                    {
                        response.IsError = true;
                        response.Message = payableTransaction.Message;
                        return response;
                    }
                    if (customer.CreditBalance > 0)
                    {
                        remainingAmount += customer.CreditBalance; // If customer has credit balance, add it to remaining amount to allocate
                    }
                    //else if (customer.CreditBalance < 0)
                    //{
                    //    // They owe you — reduce the amount available to allocate
                    //    remainingAmount -= Math.Abs(customer.CreditBalance);
                    //}

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
                    customer.CreditBalance += requestModel.Amount;
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Invalid TransactionType. Allowed values are 'Received' or 'Paid'.";
                    return response;

                }

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Message = "Payment added successfuly";

                    // response.Model = autoMapper.Map<PaymentResponseModel>(tran);

                }
                return response;
            }
        }

        public async Task<ResponseModel<PaymentResponseModel>> UpdatePayment(PaymentUpdateModel requestModel)
        {
            var response = new ResponseModel<PaymentResponseModel>();
            using var unitOfWork = unitOfWorkFactory.CreateUnitOfWork();

            var accountTransaction = await unitOfWork.AccountTransactionRepository.GetTransaction(requestModel.Id);
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
                if (customer == null)
                {
                    response.IsError = true;
                    response.Message = $"Customer not found";
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
                }

                // Update Payment info
                payment.Amount = requestModel.Amount;
                payment.PaymentDate = requestModel.PaymentDate;
                payment.PaymentMethod = requestModel.PaymentMethod;
                payment.Notes = requestModel.Notes;

                await unitOfWork.SaveChangesAsync();        // save, to get latest status of supplies. Reverse Allocations not reflect in db otherwise

                decimal remainingAmount = requestModel.Amount;

                if (accountTransaction.TransactionDirection == TransactionDirection.Credit.ToString())
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
                            //PaymentId = payment.Id,
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
                else if (accountTransaction.TransactionDirection == TransactionDirection.Debit.ToString())
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
                            // PaymentId = payment.Id,
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
                //response.Model.PaymentAllocations = new List<PaymentAllocationResponseModel>();
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

        public async Task<PaginatedResponseModel<PaymentResponseModel>> GetPayments(FilterModel filterModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<PaymentResponseModel>();

                filterModel.TransactionType = TransactionType.Payment.ToString();
                var payments = await unitOfWork.PaymentRepository.GetPayments(filterModel);

                if (payments.Model == null || payments.TotalCount < 1)
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
            if (customer == null)
            {
                response.IsError = true;
                response.Message = $"Customer not found";
                return response;
            }

            decimal allocatedTotal = 0;

            var affectedIds = new List<int>();
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
                        affectedIds.Add(order.Id);
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
                        affectedIds.Add(supply.Id);
                    }
                }

                alloc.IsActive = false;
            }

            // Reverse customer balance
            if (payment.TransactionFlow == TransactionFlow.Received.ToString())
            {
                customer.CreditBalance += payment.Amount;
            }
            else if (payment.TransactionFlow == TransactionFlow.Paid.ToString())
            {
                customer.CreditBalance -= payment.Amount;
            }

            var accountTransactions = await unitOfWork.AccountTransactionRepository.GetTransactionsByReference(ReferenceType.Payment.ToString(), payment.Id);

            foreach (var transaction in accountTransactions)
            {
                await accountTransactionService.DeleteTransaction(transaction.Id);
            }

            if (await unitOfWork.SaveChangesAsync())
            {
                response.Message = "Payment deleted successfully";
                response.Model = true;
            }

            return response;
        }

    }
}
