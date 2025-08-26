using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ServicesImpl;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuppliesController : ControllerBase
    {
        private readonly ISupplyService supplyService;

        public SuppliesController(ISupplyService supplyService)
        {
            this.supplyService = supplyService;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddSupply(SupplyAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<SupplyResponseModel>();

            response = await supplyService.AddSupply(requestModel);

            return Ok(response);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateSupply(SupplyUpdateModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<SupplyResponseModel>();

            response = await supplyService.UpdateSupply(requestModel);

            return Ok(response);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetSupplies()
        //{
        //    var supplies = await supplyService.GetAllSupplies();
        //    return Ok(supplies);
        //}

        [HttpGet("")]
        public async Task<IActionResult> GetSupplies([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? fruitId = null, [FromQuery] int? supplierId = null)
        {
            var result = await supplyService.GetSupplies(page, pageSize, fruitId, supplierId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupply(int id)
        {
            var supplies = await supplyService.GetSupply(id);
            return Ok(supplies);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupply(int id)
        {
            var response = await supplyService.DeleteSupply(id);

            return Ok(response);
        }

        [HttpGet("Unassigned")]
        public async Task<IActionResult> GetUnAssignedSupplies([FromQuery] string? truckNumber)
        {
            var result = await supplyService.GetUnassignedSupplies(truckNumber);
            return Ok(result);
        }
    }
}
