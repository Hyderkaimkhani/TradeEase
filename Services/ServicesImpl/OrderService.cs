using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.ServicesImpl
{
    public class OrderService : IOrderService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;

        public OrderService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService
            )
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.tokenService = tokenService;
        }

        //public async Task<int> CreateOrder(CreateOrderDto dto)
        //{
        //    // 1. Get or create Truck
        //    var truck = await _dbContext.Trucks
        //        .FirstOrDefaultAsync(t => t.TruckNumber == dto.TruckNumber);

        //    if (truck == null)
        //    {
        //        truck = new Truck { TruckNumber = dto.TruckNumber };
        //        _dbContext.Trucks.Add(truck);
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    // 2. Create TruckAssignment
        //    var truckAssignment = new TruckAssignment
        //    {
        //        TruckId = truck.Id,
        //        AssignmentDate = dto.DeliveryDate,
        //    };
        //    _dbContext.TruckAssignments.Add(truckAssignment);
        //    await _dbContext.SaveChangesAsync();

        //    // 3. Link unassigned supplies for this truck (if any)
        //    var supplies = await _dbContext.Supplies
        //        .Where(s => s.TruckId == truck.Id && s.TruckAssignmentId == null)
        //        .ToListAsync();

        //    foreach (var supply in supplies)
        //    {
        //        supply.TruckAssignmentId = truckAssignment.Id;
        //    }

        //    await _dbContext.SaveChangesAsync();

        //    // 4. Calculate weighted purchase price from supplies
        //    var totalPurchaseAmount = supplies
        //        .Where(s => s.Fruit == dto.Fruit)
        //        .Sum(s => s.PricePer40Kg * (s.QuantityInKg / 40));

        //    var totalSuppliedKg = supplies
        //        .Where(s => s.Fruit == dto.Fruit)
        //        .Sum(s => s.QuantityInKg);

        //    var avgPurchasePrice = totalSuppliedKg > 0
        //        ? totalPurchaseAmount / (totalSuppliedKg / 40)
        //        : 0;

        //    // 5. Create Order
        //    var order = new Order
        //    {
        //        CustomerId = dto.CustomerId,
        //        TruckAssignmentId = truckAssignment.Id,
        //        DeliveryDate = dto.DeliveryDate,
        //        Fruit = dto.Fruit,
        //        QuantityInKg = dto.QuantityInKg,
        //        SellingPricePer40Kg = dto.SellingPricePer40Kg,
        //        PurchasePricePer40Kg = avgPurchasePrice,
        //        PaymentStatus = "Pending"
        //    };

        //    _dbContext.Orders.Add(order);
        //    await _dbContext.SaveChangesAsync();

        //    return order.Id;
        //}
    }
}
