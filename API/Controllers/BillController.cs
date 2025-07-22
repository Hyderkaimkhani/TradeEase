using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillService billService;

        public BillController(IBillService billService)
        {
            this.billService = billService;
        }

        [HttpPost]
        public async Task<IActionResult> AddBill([FromBody] BillAddRequestModel model)
        {
            var result = await billService.AddBill(model);
            return result.IsError ? BadRequest(result) : Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBill(int id)
        {
            var result = await billService.GetBill(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBills([FromQuery] FilterModel filterModel)
        {
            var result = await billService.GetBills(filterModel);
            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillAddRequestModel model)
        {
            var result = await billService.UpdateBill(id, model);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var result = await billService.DeleteBill(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }
    }
}
