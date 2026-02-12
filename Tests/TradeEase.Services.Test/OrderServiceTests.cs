using AutoMapper;
using Castle.Core.Resource;
using Common;
using Domain.Entities;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.Extensions.Configuration;
using Moq;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.ServicesImpl;

namespace TradeEase.Services.Test;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IAdminService> _adminServiceMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly OrderService orderService;
    private readonly IAdminService adminService;

    public OrderServiceTests()
    {
        //_unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
        //_unitOfWorkMock = new Mock<IUnitOfWork>();
        //_tokenServiceMock = new Mock<ITokenService>();
        //_adminServiceMock = new Mock<IAdminService>();
        //_mapperMock = new Mock<IMapper>();
        //_configuration = new Mock<IConfiguration>();

        _unitOfWorkFactoryMock.Setup(f => f.CreateUnitOfWork()).Returns(_unitOfWorkMock.Object);
        //orderService = new OrderService(
        //    _unitOfWorkFactoryMock.Object,
        //    _mapperMock.Object,
        //    _configuration.Object,
        //    _tokenServiceMock.Object,
        //    _adminServiceMock.Object
        //);

        //adminService = new AdminService(
        //    _unitOfWorkFactoryMock.Object,
        //    _mapperMock.Object,
        //    _configuration.Object,
        //    _tokenServiceMock.Object
        //);
    }



    [Fact]
    public async Task AddOrder_ShouldReturnError_IfCustomerNotFound()
    {
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync((Customer?)null);

        var result = await orderService.AddOrder(new OrderAddModel { CustomerId = 1 });

        Assert.True(result.IsError);
        Assert.Contains("Customer not found", result.Message);
    }

    [Fact]
    public async Task AddOrder_ShouldReturnError_IfUnassignedSuppliesMissing()
    {
        var customer = new Customer { Id = 1, CreditBalance = 0 };
        var truck = new Truck { Id = 1, TruckNumber = "XYZ" };

        _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync(customer);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("XYZ")).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.AdminRepository.AddTruckAssignment(It.IsAny<TruckAssignment>()))
                       .ReturnsAsync(new TruckAssignment { Id = 10, TruckId = 1 });

        _unitOfWorkMock.Setup(u => u.SupplyRepository.GetUnAssignedSupplies(1))
                       .ReturnsAsync(new List<Supply>()); // Empty

        var result = await orderService.AddOrder(new OrderAddModel
        {
            CustomerId = 1,
            TruckNumber = "XYZ",
            FruitId = 1,
            OrderDate = DateTime.UtcNow
        });

        Assert.True(result.IsError);
        Assert.Contains("No unassigned supplies", result.Message);
    }

    [Fact]
    public async Task AddOrder_ShouldSucceed_WhenValid()
    {
        var customer = new Customer { Id = 1, CreditBalance = 0 };
        var truck = new Truck { Id = 1, TruckNumber = "XYZ" };
        var supply = new Supply { FruitId = 1, Quantity = 10, TotalPrice = 500 };

        var order = new Order { Id = 100, CustomerId = 1, TruckId = 1, TotalSellingPrice = 1000 };

        _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync(customer);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("XYZ")).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.AdminRepository.AddTruckAssignment(It.IsAny<TruckAssignment>()))
                       .ReturnsAsync(new TruckAssignment { Id = 10, TruckId = 1 });
        _unitOfWorkMock.Setup(u => u.SupplyRepository.GetUnAssignedSupplies(1))
                       .ReturnsAsync(new List<Supply> { supply });

        _mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderAddModel>())).Returns(order);
        _unitOfWorkMock.Setup(u => u.OrderRepository.AddOrder(It.IsAny<Order>())).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<OrderResponseModel>(It.IsAny<Order>()))
                   .Returns(new OrderResponseModel { Id = order.Id });

        _adminServiceMock.Setup(a => a.AdjustCustomerBalance(
            It.IsAny<IUnitOfWork>(), 1, 0, It.IsAny<decimal>(), OperationType.Order.ToString()))
            .Returns(Task.CompletedTask);

        var result = await orderService.AddOrder(new OrderAddModel
        {
            CustomerId = 1,
            TruckNumber = "XYZ",
            FruitId = 1,
            Quantity = 20,
            SellingPrice = 50,
            OrderDate = DateTime.UtcNow
        });

        Assert.False(result.IsError);
        Assert.Equal("Order added successfuly", result.Message);
        Assert.NotNull(result.Model);
        Assert.Equal(order.Id, result.Model.Id);
    }

    [Fact]
    public async Task AddOrder_ShouldAutoAllocatePayment_WhenAdvanceBalanceExists()
    {
        var customer = new Customer { Id = 1, CreditBalance = -500 };
        var truck = new Truck { Id = 1 };
        var supply = new Supply { FruitId = 1, Quantity = 10, TotalPrice = 300 };
        var order = new Order { Id = 200, CustomerId = 1, TotalSellingPrice = 600 };

        _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync(customer);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck(It.IsAny<string>())).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.SupplyRepository.GetUnAssignedSupplies(1))
                       .ReturnsAsync(new List<Supply> { supply });
        _unitOfWorkMock.Setup(u => u.AdminRepository.AddTruckAssignment(It.IsAny<TruckAssignment>()))
                       .ReturnsAsync(new TruckAssignment { Id = 12, TruckId = 1 });
        _mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderAddModel>())).Returns(order);
        _unitOfWorkMock.Setup(u => u.OrderRepository.AddOrder(It.IsAny<Order>())).ReturnsAsync(order);

        _unitOfWorkMock.Setup(u => u.PaymentRepository.AddPayment(It.IsAny<Payment>()))
                       .ReturnsAsync(new Payment { Id = 300 });

        _unitOfWorkMock.Setup(u => u.PaymentRepository.AddPaymentAllocation(It.IsAny<PaymentAllocation>()))
                       .ReturnsAsync(new PaymentAllocation());

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<OrderResponseModel>(It.IsAny<Order>()))
                   .Returns(new OrderResponseModel { Id = order.Id });

        _adminServiceMock.Setup(a => a.AdjustCustomerBalance(It.IsAny<IUnitOfWork>(), 1, 0, 600, "Order"))
            .Returns(Task.CompletedTask);

        var result = await orderService.AddOrder(new OrderAddModel
        {
            CustomerId = 1,
            TruckNumber = "TRUCK1",
            FruitId = 1,
            Quantity = 12,
            SellingPrice = 50,
            OrderDate = DateTime.UtcNow
        });
        customer = adminService.AdjustCustomerBalance(customer, 0, 600, "Order");
        Assert.Equal(100, customer.CreditBalance); // -500 + 600

        Assert.False(result.IsError);
        Assert.Equal("Order added successfuly", result.Message);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnError_IfOrderNotFound()
    {
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync((Order?)null);

        var result = await orderService.UpdateOrder(new OrderUpdateModel { Id = 1 });

        Assert.True(result.IsError);
        Assert.Equal("No Order found", result.Message);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnError_IfTruckNotFound()
    {
        var order = new Order { Id = 1, PaymentStatus = PaymentStatus.Unpaid.ToString() };
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("TRK-001")).ReturnsAsync((Truck?)null);

        var result = await orderService.UpdateOrder(new OrderUpdateModel
        {
            Id = 1,
            TruckNumber = "TRK-001"
        });

        Assert.True(result.IsError);
        Assert.Equal("Truck not exists. Please add Truck first and assign Supply", result.Message);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnError_IfPaymentMadeAndQuantityOrPriceChanged()
    {
        var order = new Order
        {
            Id = 1,
            PaymentStatus = PaymentStatus.Paid.ToString(),
            Quantity = 10,
            SellingPrice = 100,
            Truck = new Truck { TruckNumber = "TRK-001" }
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("TRK-001")).ReturnsAsync(order.Truck);

        var result = await orderService.UpdateOrder(new OrderUpdateModel
        {
            Id = 1,
            Quantity = 20, // changed
            SellingPrice = 100, // same
            TruckNumber = "TRK-001"
        });

        Assert.True(result.IsError);
        Assert.Contains("payment already made", result.Message);
    }

    [Fact]
    public async Task UpdateOrder_ShouldReturnError_IfSaveChangesFails()
    {
        var truck = new Truck { Id = 1, TruckNumber = "TRK-001" };
        var order = new Order
        {
            Id = 1,
            PaymentStatus = PaymentStatus.Unpaid.ToString(),
            Quantity = 5,
            SellingPrice = 50,
            TotalSellingPrice = 250,
            Truck = truck
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("TRK-001")).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(false);

        var result = await orderService.UpdateOrder(new OrderUpdateModel
        {
            Id = 1,
            Quantity = 5,
            SellingPrice = 50,
            TruckNumber = "TRK-001"
        });

        Assert.True(result.IsError);
        Assert.Equal("Unable to update Order.", result.Message);
    }

    [Fact]
    public async Task UpdateOrder_ShouldUpdateOrderAndCallAdjustCustomerBalance_WhenTotalSellingPriceChanges()
    {
        // Arrange
        var oldTotal = 1000m;
        var newQuantity = 8;
        var newPrice = 150m;
        var newTotal = newQuantity * newPrice; // 1200

        var customer = new Customer
        {
            Id = 123,
            CreditBalance = 5000
        };

        var truck = new Truck { Id = 1, TruckNumber = "TRK-001" };
        var order = new Order
        {
            Id = 1,
            CustomerId = 123,
            PaymentStatus = PaymentStatus.Unpaid.ToString(),
            Quantity = 10,
            SellingPrice = 100,
            TotalSellingPrice = 1000,
            Truck = truck
        };

        var updateModel = new OrderUpdateModel
        {
            Id = 1,
            TruckNumber = "TRK-001",
            Quantity = newQuantity,
            SellingPrice = newPrice,
            FruitId = 2,
            DeliveryDate = DateTime.UtcNow.AddDays(2)
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck(truck.TruckNumber)).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<OrderResponseModel>(It.IsAny<Order>())).Returns(new OrderResponseModel());

        decimal capturedOld = 0, capturedNew = 0;
        _adminServiceMock.Setup(a => a.AdjustCustomerBalance(
                It.IsAny<IUnitOfWork>(),
                order.CustomerId,
                It.IsAny<decimal>(),
                It.IsAny<decimal>(),
                OperationType.Order.ToString()))
            .Callback<IUnitOfWork, int, decimal, decimal, string>((_, __, oldVal, newVal, ___) =>
            {
                capturedOld = oldVal;
                capturedNew = newVal;
            })
            .Returns(Task.CompletedTask);

        var updatedCustomer = adminService.AdjustCustomerBalance(customer, oldTotal, newTotal, "Order");

        var expectedBalance = 5000 + (newTotal - oldTotal); // 5000 + (1200 - 1000)
        Assert.Equal(expectedBalance, updatedCustomer.CreditBalance);
        // Act
        var result = await orderService.UpdateOrder(updateModel);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Order updated successfully.", result.Message);
        Assert.Equal(1000, capturedOld);
        Assert.Equal(8 * 150, capturedNew); // new total: 1200
    }

    [Fact]
    public async Task UpdateOrder_ShouldCallAdjustCustomerBalance_WhenQuantityOrPriceChanges()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 123,
            CreditBalance = 5000
        };

        var originalTotal = 1000; // 10 * 100
        var newQuantity = 15;
        var newPrice = 80;
        var expectedNewTotal = newQuantity * newPrice; // 1200

        var truck = new Truck { Id = 2, TruckNumber = "TRK-001" };

        var existingOrder = new Order
        {
            Id = 1,
            CustomerId = 5,
            TruckId = 2,
            Truck = truck,
            PaymentStatus = PaymentStatus.Unpaid.ToString(),
            Quantity = 10,
            SellingPrice = 100,
            TotalSellingPrice = originalTotal,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Deliverydate = DateTime.UtcNow.AddDays(1)
        };

        var updateModel = new OrderUpdateModel
        {
            Id = 1,
            Quantity = newQuantity,
            SellingPrice = newPrice,
            TruckNumber = "TRK-001",
            FruitId = 7,
            DeliveryDate = DateTime.UtcNow.AddDays(2)
        };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrder(1)).ReturnsAsync(existingOrder);
        _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("TRK-001")).ReturnsAsync(truck);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<OrderResponseModel>(It.IsAny<Order>())).Returns(new OrderResponseModel());

        decimal capturedOldAmount = 0, capturedNewAmount = 0;
        _adminServiceMock.Setup(a =>
                a.AdjustCustomerBalance(
                    It.IsAny<IUnitOfWork>(),
                    existingOrder.CustomerId,
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    OperationType.Order.ToString()))
            .Callback<IUnitOfWork, int, decimal, decimal, string>((_, __, oldAmt, newAmt, ___) =>
            {
                capturedOldAmount = oldAmt;
                capturedNewAmount = newAmt;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await orderService.UpdateOrder(updateModel);

        var updatedCustomer = adminService.AdjustCustomerBalance(customer, capturedOldAmount, capturedNewAmount, "Order");

        var expectedBalance = 5000 + (capturedNewAmount - capturedOldAmount); // 5000 + (1200 - 1000)
        Assert.Equal(expectedBalance, updatedCustomer.CreditBalance);
        // Assert
        Assert.False(result.IsError);
        Assert.Equal("Order updated successfully.", result.Message);

        // Confirm AdjustCustomerBalance called with correct values
        Assert.Equal(originalTotal, capturedOldAmount);
        Assert.Equal(expectedNewTotal, capturedNewAmount);

        // Confirm updated order values
        Assert.Equal(newQuantity, existingOrder.Quantity);
        Assert.Equal(newPrice, existingOrder.SellingPrice);
        Assert.Equal(expectedNewTotal, existingOrder.TotalSellingPrice);
    }
}


