using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;

namespace Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ResponseModel<CompanyResponseModel>> AddCompany(CompanyAddRequestModel model);
        Task<ResponseModel<CompanyResponseModel>> GetCompany(int id);
        Task<ResponseModel<List<CompanyResponseModel>>> GetAllCompanies();
        Task<ResponseModel<CompanyResponseModel>> UpdateCompany(CompanyUpdateRequestModel model);
        Task<ResponseModel<bool>> DeleteCompany(int id);
    }
}
