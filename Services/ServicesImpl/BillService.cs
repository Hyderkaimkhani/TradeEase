using AutoMapper;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.ServicesImpl
{
    public class BillService : IBillService
    {
        private readonly IBillRepository billRepository;
        private readonly IMapper mapper;

        public BillService(IBillRepository billRepository, IMapper mapper)
        {
            this.billRepository = billRepository;
            this.mapper = mapper;
        }

        public async Task<ResponseModel<BillResponseModel>> AddBill(BillAddRequestModel model)
        {
            var bill = mapper.Map<Bill>(model);
            bill.IsActive = true;
            var added = await billRepository.AddBill(bill);
            return new ResponseModel<BillResponseModel>
            {
                Model = mapper.Map<BillResponseModel>(added),
                Message = "Bill created successfully"
            };
        }

        public async Task<ResponseModel<BillResponseModel>> GetBill(int id)
        {
            var bill = await billRepository.GetBill(id);
            if (bill == null)
                return new ResponseModel<BillResponseModel> { IsError = true, Message = "Bill not found" };

            return new ResponseModel<BillResponseModel>
            {
                Model = mapper.Map<BillResponseModel>(bill)
            };
        }

        public async Task<ResponseModel<List<BillResponseModel>>> GetBills(int? entityId, string? entityType)
        {
            var bills = await billRepository.GetBills(entityId, entityType);
            return new ResponseModel<List<BillResponseModel>>
            {
                Model = mapper.Map<List<BillResponseModel>>(bills)
            };
        }

        public async Task<ResponseModel<BillResponseModel>> UpdateBill(int id, BillAddRequestModel model)
        {
            var bill = await billRepository.GetBill(id);
            if (bill == null)
                return new ResponseModel<BillResponseModel> { IsError = true, Message = "Bill not found" };

            mapper.Map(model, bill);
            await billRepository.UpdateBill(bill);

            return new ResponseModel<BillResponseModel>
            {
                Model = mapper.Map<BillResponseModel>(bill),
                Message = "Bill updated successfully"
            };
        }

        public async Task<ResponseModel<bool>> DeleteBill(int id)
        {
            var result = await billRepository.DeleteBill(id);
            return new ResponseModel<bool>
            {
                Model = result,
                Message = result ? "Bill deleted successfully" : "Bill not found",
                IsError = !result
            };
        }
    }
}
