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
    public class OrderService : IOrderService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;
        private readonly IAdminService adminService;

        public OrderService(IUnitOfWorkFactory unitOfWorkFactory,
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

        public async Task<ResponseModel<OrderResponseModel>> AddOrder(OrderAddModel requestModel)
        {
            var user = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<OrderResponseModel>();

                // 1. Get or create Truck
                Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);

                // I think If truck not exists , should not create Order
                if (truck == null)
                {
                    var truckEntity = new Truck
                    {
                        TruckNumber = requestModel.TruckNumber,
                        CreatedBy = user,
                        UpdatedBy = user,
                        IsActive = true
                    };
                    truck = await unitOfWork.AdminRepository.AddTruck(truckEntity);
                    await unitOfWork.SaveChangesAsync();
                }

                // 2. Create TruckAssignment
                var truckAssignment = new TruckAssignment
                {
                    TruckId = truck.Id,
                    AssignedDate = requestModel.OrderDate,
                };
                truckAssignment = await unitOfWork.AdminRepository.AddTruckAssignment(truckAssignment);


                // 3. Link unassigned supplies for this truck (if any)
                List<Supply> supplies = await unitOfWork.SupplyRepository.GetUnAssignedSupplies(truck.Id);
                if (supplies == null || supplies.Count < 1)
                {
                    response.IsError = true;
                    response.Message = "No unassigned supplies found for this truck. Kindly add a Supply against this Order";
                    return response;
                }

                foreach (var supply in supplies)
                {
                    supply.TruckAssignmentId = truckAssignment.Id;
                }

                // 4. Calculate weighted purchase price from supplies
                var totalPurchaseAmount = supplies
                    .Where(s => s.FruitId == requestModel.FruitId)
                    .Sum(s => s.TotalPrice);

                var totalSupplied = supplies
                    .Where(s => s.FruitId == requestModel.FruitId)
                    .Sum(s => s.Quantity);

                var avgPurchasePrice = totalSupplied > 0
                    ? totalPurchaseAmount / totalSupplied
                    : 0;

                // 5. Create Order
                var order = autoMapper.Map<Order>(requestModel);

                order.PurchasePrice = avgPurchasePrice;
                order.TotalPurchaseAmount = totalPurchaseAmount;
                order.TotalSellingAmount = requestModel.Quantity * requestModel.SellingPrice;
                order.ProfitLoss = order.TotalSellingAmount - order.TotalPurchaseAmount;
                order.TruckAssignmentId = truckAssignment.Id;

                order.TruckId = truck.Id;
                order.IsActive = true;
                order.CreatedBy = user;
                order.UpdatedBy = user;

                var addedOrder = await unitOfWork.OrderRepository.AddOrder(order);
                if (addedOrder != null)
                {
                    await adminService.AdjustCustomerBalance(unitOfWork, order.CustomerId, 0, order.TotalSellingAmount, OperationType.Order.ToString());
                    //var customer = await unitOfWork.AdminRepository.GetCustomer(requestModel.CustomerId);
                    //if (customer != null)
                    //{
                    //    customer.CreditBalance += order.TotalSellingAmount; // Receiveable to Customer (+ve)
                    //    customer.UpdatedBy = user;
                    //}
                    //else
                    //{
                    //    response.IsError = true;
                    //    response.Message = "Customer not exists.";
                    //}

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Order added successfuly";
                        response.Model = autoMapper.Map<OrderResponseModel>(addedOrder);
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add Order";
                    }

                }
                else
                {
                    response.IsError = true;
                    response.Message = "Unable to add Order";
                }

                return response;
            }

        }

        public async Task<ResponseModel<OrderResponseModel>> UpdateOrder(OrderUpdateModel requestModel)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                // Restrict to change price and quantity if payment made against this Order.
                var response = new ResponseModel<OrderResponseModel>();

                var order = await unitOfWork.OrderRepository.GetOrder(requestModel.Id);

                if (order == null)
                {
                    response.IsError = true;
                    response.Message = "No Order found";
                }
                else
                {
                    Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);

                    if (truck == null)
                    {
                        response.IsError = true;
                        response.Message = "Truck not exists. Please add Truck first and its Supply";
                        return response;
                    }

                    decimal oldTotal = order.TotalSellingAmount;

                    order.FruitId = requestModel.FruitId;
                    order.SellingPrice = requestModel.SellingPrice;
                    order.Quantity = requestModel.Quantity;
                    order.TruckId = truck.Id;
                    order.TotalSellingAmount = requestModel.Quantity * requestModel.SellingPrice;
                    order.OrderDate = requestModel.OrderDate;
                    order.Deliverydate = requestModel.DeliveryDate;
                    order.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    decimal difference = oldTotal - order.TotalSellingAmount;  // 10-8=2, 8-10 = -2

                    if (oldTotal != order.TotalSellingAmount)
                    {
                        // Adjust Customer Account
                        await adminService.AdjustCustomerBalance(unitOfWork, order.CustomerId, oldTotal, order.TotalSellingAmount, OperationType.Order.ToString());
                    }

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Message = "Order updated successfully.";
                        response.Model = autoMapper.Map<OrderResponseModel>(order);
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to update Order.";
                    }
                    // handle case if current CreditBalance is negative then adjust for this order, means you owe for customer


                    // Optionally recalculate payment status if using derived AmountReceived
                    //await UpdateOrderPaymentStatusAsync(order.Id);
                }
                return response;
            }
        }

        public async Task<ResponseModel<OrderResponseModel>> GetOrder(int id)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<OrderResponseModel>();

                var order = await unitOfWork.OrderRepository.GetOrder(id);

                if (order == null)
                {
                    response.IsError = true;
                    response.Message = "Order does not exists";
                }
                else
                {
                    response.Model = autoMapper.Map<OrderResponseModel>(order);
                }

                return response;
            }
        }

        public async Task<PaginatedResponseModel<OrderResponseModel>> GetOrders(int page, int pageSize, int? fruitId, int? customerId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new PaginatedResponseModel<OrderResponseModel>();

                var orders = await unitOfWork.OrderRepository.GetOrders(page, pageSize, fruitId, customerId);

                if (orders.Model == null || orders.Model.Count < 1)
                {
                    response.Message = "No Order found";
                    response.Model = new List<OrderResponseModel>();
                }
                else
                {
                    response.Model = autoMapper.Map<List<OrderResponseModel>>(orders.Model);
                    response.TotalCount = orders.Model.Count;
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> DeleteOrder(int orderId)
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<bool>();
                var order = await unitOfWork.OrderRepository.GetOrder(orderId);

                // Optional: Check if payment is allocated, prevent delete if necessary
                //var hasPayments = await _context.PaymentAllocations
                //    .AnyAsync(p => p.OrderId == orderId);

                //if (hasPayments)
                //    throw new InvalidOperationException("Cannot delete order with allocated payments.");

                if (order == null || !order.IsActive)
                {
                    response.IsError = true;
                    response.Message = "Order does not exists";
                }
                else
                {
                    await adminService.AdjustCustomerBalance(unitOfWork, order.CustomerId, order.TotalSellingAmount, 0, OperationType.Order.ToString());
                    order.IsActive = false;
                    order.UpdatedDate = DateTime.UtcNow;
                    order.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Order deleted succesfully.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to delete Order";
                        response.Model = false;
                    }
                }
                return response;
            }
        }

        public async Task UpdateOrderPaymentStatusAsync(int orderId)
        {
            //var order = await _context.Orders.FindAsync(orderId);
            //if (order == null) return;

            //var totalReceived = await _context.PaymentAllocations
            //    .Where(p => p.OrderId == orderId)
            //    .SumAsync(p => p.AllocatedAmount);

            //order.AmountReceived = totalReceived;

            //if (totalReceived == 0)
            //    order.PaymentStatus = "Unpaid";
            //else if (totalReceived < order.TotalAmount)
            //{
            //    order.PaymentStatus = "Partial";
            //    order.OverpaidAmount = 0;
            //}
            //else if (totalReceived == order.TotalAmount)
            //{
            //    order.PaymentStatus = "Paid";
            //    order.OverpaidAmount = 0;
            //}
            //else // Overpaid
            //{
            //    order.PaymentStatus = "Paid";
            //    order.OverpaidAmount = totalReceived - order.TotalAmount;

            //    // Log credit to customer account for adjustment
            //    await _customerAccountService.CreditCustomer(order.CustomerId, order.OverpaidAmount);
            //}

            //await _context.SaveChangesAsync();
        }

    }
}
