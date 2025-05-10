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
    public class SupplyService : ISupplyService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITokenService tokenService;

        public SupplyService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService
            )
        {
            this.autoMapper = autoMapper;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.tokenService = tokenService;
        }

        public async Task<ResponseModel<SupplyResponseModel>> AddSupply(SupplyAddRequest requestModel)
        {
            var user = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<SupplyResponseModel>();

                // Get Truck if not exists add
                Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);

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
                }

                var supply = autoMapper.Map<Supply>(requestModel);

                supply.TotalPrice = requestModel.Quantity * requestModel.PurchasePrice;
                supply.PaymentStatus = requestModel.AmountPaid == 0
                    ? "Unpaid"
                    : (requestModel.AmountPaid < supply.TotalPrice ? "Partial" : "Paid");

                supply.TruckId = truck.Id;
                supply.IsActive = true;
                supply.CreatedBy = user;
                supply.UpdatedBy = user;

                var addedSupply = await unitOfWork.SupplyRepository.AddSupply(supply);
                if (addedSupply != null)
                {
                    await unitOfWork.SaveChangesAsync();
                    response.Message = "Supply added successfuly";
                    response.Model = autoMapper.Map<SupplyResponseModel>(addedSupply);

                }
                else
                {
                    response.IsError = true;
                    response.Message = "Unable to add Supply";
                }

                return response;
            }
        }



        public async Task<ResponseModel<SupplyResponseModel>> UpdateSupply(SupplyUpdateRequest requestModel)
        {
            var user = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
            var response = new ResponseModel<SupplyResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {

                var supply = await unitOfWork.SupplyRepository.GetSupply(requestModel.Id);

                supply = autoMapper.Map<Supply>(requestModel);

                supply.TotalPrice = requestModel.Quantity * requestModel.PurchasePrice;
                supply.PaymentStatus = requestModel.AmountPaid == 0
                    ? "Unpaid"
                    : (requestModel.AmountPaid < supply.TotalPrice ? "Partial" : "Paid");

                Truck? truck = await unitOfWork.AdminRepository.GetTruck(requestModel.TruckNumber);

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
                }
                supply.TruckId = truck.Id;

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
            return response;
        }

        public async Task<ResponseModel<List<SupplyResponseModel>>> GetAllSupplies()
        {
            var user = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
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
            var user = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
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
                    response.TotalCount = supplies.Model.Count;
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
                    supply.IsActive = false;
                    supply.UpdatedDate = DateTime.Now;
                    supply.UpdatedBy = await tokenService.GetClaimFromToken(ClaimType.Custom_Sub);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Customer deleted Supply.";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to delete Supply";
                        response.Model = false;
                    }
                }
                return response;
            }
        }

    }
}
