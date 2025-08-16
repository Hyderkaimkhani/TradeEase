using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ServicesImpl;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddPayment(PaymentAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await paymentService.AddPayment(requestModel);

            return Ok(response);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPayments([FromQuery] FilterModel filterModel)
        {
            var result = await paymentService.GetPayments(filterModel);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var result = await paymentService.GetPayment(id);
            return Ok(result);
        }

        [HttpPost("Update")]
        [Obsolete]
        public async Task<IActionResult> UpdatePayment(PaymentUpdateModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await paymentService.UpdatePayment(requestModel);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var response = await paymentService.DeletePayment(id);

            return Ok(response);
        }
    }
}
