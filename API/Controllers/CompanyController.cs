using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] CompanyAddRequestModel model)
        {
            var result = await companyService.AddCompany(model);
            return result.IsError ? BadRequest(result) : Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var result = await companyService.GetCompany(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var result = await companyService.GetAllCompanies();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyUpdateRequestModel model)
        {
            var result = await companyService.UpdateCompany(model);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var result = await companyService.DeleteCompany(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }
    }
}
