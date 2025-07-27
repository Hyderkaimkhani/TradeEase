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

namespace TradeEase.Services.Test
{
    public class SupplyServiceTests
    {
        private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configuration;
        private readonly SupplyService _supplyService;
        private readonly IAdminService adminService;

        public SupplyServiceTests()
        {
            _unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _adminServiceMock = new Mock<IAdminService>();
            _mapperMock = new Mock<IMapper>();
            _configuration = new Mock<IConfiguration>();

            _unitOfWorkFactoryMock.Setup(f => f.CreateUnitOfWork()).Returns(_unitOfWorkMock.Object);
            _supplyService = new SupplyService(
                _unitOfWorkFactoryMock.Object,
                _mapperMock.Object,
                _configuration.Object,
                _tokenServiceMock.Object,
                _adminServiceMock.Object
            );

            adminService = new AdminService(
                _unitOfWorkFactoryMock.Object,
                _mapperMock.Object,
                _configuration.Object,
                _tokenServiceMock.Object
            );

        }

        [Fact]
        public async Task AddSupply_ShouldFail_IfSupplierNotFound()
        {
            // Arrange
            var model = new SupplyAddModel { SupplierId = 1 };
            _tokenServiceMock.Setup(t => t.GetClaimFromToken(It.IsAny<string>())).ReturnsAsync("test-user");
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _supplyService.AddSupply(model);

            // Assert
            Assert.True(result.IsError);
            Assert.Contains("Supplier not found", result.Message);
        }

        [Fact]
        public async Task AddSupply_ShouldCreateTruck_IfNotExists()
        {
            // Arrange
            var model = new SupplyAddModel
            {
                SupplierId = 1,
                TruckNumber = "ABC123",
                Quantity = 10,
                PurchasePrice = 50
            };

            _tokenServiceMock.Setup(t => t.GetClaimFromToken(It.IsAny<string>())).ReturnsAsync("user");
            var supplier = new Customer { Id = 1, CreditBalance = 0 };
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync(supplier);
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("ABC123")).ReturnsAsync((Truck?)null);
            _unitOfWorkMock.Setup(u => u.AdminRepository.AddTruck(It.IsAny<Truck>())).ReturnsAsync(new Truck { Id = 99 });
            _mapperMock.Setup(m => m.Map<Supply>(model)).Returns(new Supply { SupplierId = 1 });
            _unitOfWorkMock.Setup(u => u.SupplyRepository.AddSupply(It.IsAny<Supply>())).ReturnsAsync(new Supply());
            //_unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(true)).ReturnsAsync(true);


            _adminServiceMock.Setup(a => a.AdjustCustomerBalance(It.IsAny<IUnitOfWork>(), 1, 0, 500, "Supply"))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _supplyService.AddSupply(model);

            // Assert
            Assert.False(result.IsError);
            Assert.Contains("Supply added", result.Message);
        }

        [Fact]
        public async Task AddSupply_ShouldAutoAllocateCreditBalance()
        {
            // Arrange
            var model = new SupplyAddModel
            {
                SupplierId = 1,
                TruckNumber = "XYZ",
                Quantity = 10,
                PurchasePrice = 100
            };

            decimal totalPrice =  model.Quantity * model.PurchasePrice; // TotalPrice = 10 * 100 = 1000
            var supplier = new Customer { Id = 1, CreditBalance = 300 };
            var truck = new Truck { Id = 1 };
            var supply = new Supply { Id = 10, SupplierId = 1 };

            _tokenServiceMock.Setup(t => t.GetClaimFromToken(It.IsAny<string>())).ReturnsAsync("user");
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetCustomer(1)).ReturnsAsync(supplier);
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("XYZ")).ReturnsAsync(truck);
            _mapperMock.Setup(m => m.Map<Supply>(It.IsAny<SupplyAddModel>())).Returns(supply);
            _unitOfWorkMock.Setup(u => u.SupplyRepository.AddSupply(It.IsAny<Supply>())).ReturnsAsync(supply);
            _unitOfWorkMock.Setup(u => u.PaymentRepository.AddPayment(It.IsAny<Payment>())).ReturnsAsync(new Payment { Id = 1 });
            _unitOfWorkMock.Setup(u => u.PaymentRepository.AddPaymentAllocation(It.IsAny<PaymentAllocation>())).ReturnsAsync(new PaymentAllocation { Id = 1 });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(true)).ReturnsAsync(true);
            _adminServiceMock.Setup(a => a.AdjustCustomerBalance(It.IsAny<IUnitOfWork>(), 1, 0, totalPrice, "Supply"))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _supplyService.AddSupply(model);

            supplier =  adminService.AdjustCustomerBalance(supplier, 0, totalPrice, "Supply");
            //var customer = await _adminService.GetCustomer(1);
            Assert.Equal(-700, supplier.CreditBalance); // 300 - 1000

            // Assert
            Assert.False(result.IsError);
            Assert.Contains("Supply added", result.Message);
        }

        // region Update Supply

        [Fact]
        public async Task UpdateSupply_ShouldReturnError_IfSupplyNotFound()
        {
            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(It.IsAny<int>())).ReturnsAsync((Supply?)null);

            var result = await _supplyService.UpdateSupply(new SupplyUpdateModel { Id = 1 });

            Assert.True(result.IsError);
            Assert.Equal("Supply does not exists.", result.Message);
        }

        [Fact]
        public async Task UpdateSupply_ShouldReturnError_IfPaymentAlreadyMade_AndQuantityOrPriceChanges()
        {
            var existingSupply = new Supply
            {
                Id = 1,
                Quantity = 5,
                PurchasePrice = 10,
                PaymentStatus = PaymentStatus.Partial.ToString()
            };

            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(1)).ReturnsAsync(existingSupply);

            var result = await _supplyService.UpdateSupply(new SupplyUpdateModel
            {
                Id = 1,
                Quantity = 6, // changed
                PurchasePrice = 12 // changed
            });

            Assert.True(result.IsError);
            Assert.Contains("payment already made", result.Message);
        }

        [Fact]
        public async Task UpdateSupply_ShouldReturnError_IfTruckIsAssigned()
        {
            var existingSupply = new Supply
            {
                Id = 1,
                Truck = new Truck { TruckNumber = "ABC123" },
                TruckAssignmentId = 99,
                PaymentStatus = PaymentStatus.Unpaid.ToString()
            };

            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(1)).ReturnsAsync(existingSupply);

            var result = await _supplyService.UpdateSupply(new SupplyUpdateModel
            {
                Id = 1,
                TruckNumber = "XYZ123", // new truck number
                Quantity = 5,
                PurchasePrice = 10
            });

            Assert.True(result.IsError);
            Assert.Contains("Truck already assigned", result.Message);
        }

        [Fact]
        public async Task UpdateSupply_ShouldAddNewTruck_IfNotExists()
        {
            var existingSupply = new Supply
            {
                Id = 1,
                Quantity = 5,
                PurchasePrice = 10,
                Truck = new Truck { TruckNumber = "OLD123" },
                PaymentStatus = PaymentStatus.Unpaid.ToString()
            };

            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(1)).ReturnsAsync(existingSupply);
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("NEW123")).ReturnsAsync((Truck?)null);
            _unitOfWorkMock.Setup(u => u.AdminRepository.AddTruck(It.IsAny<Truck>()))
                           .ReturnsAsync(new Truck { Id = 101, TruckNumber = "NEW123" });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);

            _adminServiceMock.Setup(s => s.AdjustCustomerBalance(
                    It.IsAny<IUnitOfWork>(),
                    It.IsAny<int>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    OperationType.Supply.ToString()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<SupplyResponseModel>(It.IsAny<Supply>())).Returns(new SupplyResponseModel());

            var result = await _supplyService.UpdateSupply(new SupplyUpdateModel
            {
                Id = 1,
                TruckNumber = "NEW123",
                Quantity = 5,
                PurchasePrice = 10
            });

            Assert.False(result.IsError);
            Assert.Equal("Supply updated successfully.", result.Message);
        }

        [Fact]
        public async Task UpdateSupply_ShouldReturnError_IfSaveFails()
        {
            var existingSupply = new Supply
            {
                Id = 1,
                Quantity = 5,
                PurchasePrice = 10,
                Truck = new Truck { TruckNumber = "ABC123" },
                PaymentStatus = PaymentStatus.Unpaid.ToString()
            };

            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(1)).ReturnsAsync(existingSupply);
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("ABC123")).ReturnsAsync(new Truck { Id = 5 });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(false);

            var result = await _supplyService.UpdateSupply(new SupplyUpdateModel
            {
                Id = 1,
                Quantity = 5,
                PurchasePrice = 10,
                TruckNumber = "ABC123"
            });

            Assert.True(result.IsError);
            Assert.Equal("Unable to update Supply.", result.Message);
        }

        [Fact]
        public async Task UpdateSupply_ShouldCallAdjustCustomerBalance_WhenQuantityOrPriceChanges()
        {
            // Arrange
            var originalTotalPrice = 500; // 10 qty * 50 price
            var newQuantity = 20;
            var newPrice = 30; // new total = 600
            var expectedNewTotal = newQuantity * newPrice;

            var supplier = new Customer
            {
                Id = 1,
                CreditBalance = 1000 // for assertion tracking
            };

            var truck = new Truck { Id = 1, TruckNumber = "TRK-123" };

            var supply = new Supply
            {
                Id = 10,
                SupplierId = 1,
                Truck = truck,
                Quantity = 10,
                PurchasePrice = 50,
                TotalPrice = originalTotalPrice,
                TruckAssignmentId = null,
                PaymentStatus = PaymentStatus.Unpaid.ToString()
            };

            var updateModel = new SupplyUpdateModel
            {
                Id = 10,
                Quantity = newQuantity,
                PurchasePrice = newPrice,
                TruckNumber = "TRK-123",
                SupplyDate = DateTime.UtcNow,
                Notes = "Updated supply"
            };

            _unitOfWorkMock.Setup(u => u.SupplyRepository.GetSupply(updateModel.Id)).ReturnsAsync(supply);
            _unitOfWorkMock.Setup(u => u.AdminRepository.GetTruck("TRK-123")).ReturnsAsync(truck);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<SupplyResponseModel>(It.IsAny<Supply>())).Returns(new SupplyResponseModel());

            // Capture the AdjustCustomerBalance call
            decimal capturedOldAmount = 0;
            decimal capturedNewAmount = 0;

            _adminServiceMock
                .Setup(a => a.AdjustCustomerBalance(
                    It.IsAny<IUnitOfWork>(),
                    supply.SupplierId,
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    OperationType.Supply.ToString()))
                .Callback<IUnitOfWork, int, decimal, decimal, string>((_, __, oldAmt, newAmt, ___) =>
                {
                    capturedOldAmount = oldAmt;
                    capturedNewAmount = newAmt;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _supplyService.UpdateSupply(updateModel);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("Supply updated successfully.", result.Message);

            Assert.Equal(originalTotalPrice, capturedOldAmount);
            Assert.Equal(expectedNewTotal, capturedNewAmount);

            // Simulate AdjustCustomerBalance effect
            var newBalance = supplier.CreditBalance - (capturedNewAmount - capturedOldAmount); // 1000 - (600-500)
            Assert.Equal(900, newBalance);
        }
    }

}
