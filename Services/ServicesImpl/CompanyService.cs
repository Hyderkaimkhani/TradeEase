using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Drawing;

namespace Services.ServicesImpl
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IMapper mapper;

        public CompanyService(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.mapper = mapper;
        }

        public async Task<ResponseModel<CompanyResponseModel>> AddCompany(CompanyAddRequestModel model)
        {

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<CompanyResponseModel>();
                byte[]? logoBytes = null;

                if (model.Logo != null && model.Logo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await model.Logo.CopyToAsync(memoryStream);

                    // Optional: restrict size (1 MB)
                    if (memoryStream.Length > 1024 * 1024)
                    {
                        response.IsError = true;
                        response.Message = "Logo size exceeds the maximum limit of 1 MB.";
                        return response;
                    }
                    logoBytes = Utilities.ResizeImage(memoryStream.ToArray(), 300, 100);
                }
                var company = mapper.Map<Company>(model);
                company.Logo = logoBytes;
                company.Code = Utilities.GenerateRandomNumberULID();
                company.MaxUsers = 5;
                company.CurrencySymbol = "Rs";
                company.Timezone = "Asia/Karachi | +05:00";
                company.Logo = model.Logo != null ? logoBytes : null;
                company.IsActive = true;
                var added = await unitOfWork.CompanyRepository.AddCompany(company);

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = mapper.Map<CompanyResponseModel>(added);
                    response.Message = "Company created successfully";
                }
                return response;
            }
        }

        public async Task<ResponseModel<CompanyResponseModel>> GetCompany(int id)
        {
            var response = new ResponseModel<CompanyResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var company = await unitOfWork.CompanyRepository.GetCompany(id);
                if (company == null)
                    return new ResponseModel<CompanyResponseModel> { IsError = true, Message = "Company not found" };

                response.Model = mapper.Map<CompanyResponseModel>(company);
                return response;
            }
        }

        public async Task<ResponseModel<List<CompanyResponseModel>>> GetAllCompanies()
        {
            var response = new ResponseModel<List<CompanyResponseModel>>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var companies = await unitOfWork.CompanyRepository.GetAllCompanies();
                response.Model = mapper.Map<List<CompanyResponseModel>>(companies);

                return response;
            }
        }

        public async Task<ResponseModel<CompanyResponseModel>> UpdateCompany(CompanyUpdateRequestModel model)
        {
            var response = new ResponseModel<CompanyResponseModel>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var company = await unitOfWork.CompanyRepository.GetCompany(model.Id);
                if (company == null)
                {
                    response.Message = "Company not found";
                    response.IsError = true;
                    return response;
                }

                company.Name = model.Name;
                company.Address = model.Address;
                company.Phone = model.Phone;
                company.Email = model.Email;

                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = mapper.Map<CompanyResponseModel>(company);
                    response.Message = "Company updated successfully";
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> DeleteCompany(int id)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var company = await unitOfWork.CompanyRepository.GetCompany(id);
                if (company == null)
                {
                    response.Message = "Company not found";
                    response.IsError = true;
                    return response;
                }
                company.IsActive = false;
                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = true;
                    response.Message = "Company deleted successfully";
                }
                return response;
            }
        }

        private async Task<byte[]> ConvertToBytes(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

    }
}
