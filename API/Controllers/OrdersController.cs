using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddOrder(OrderAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<OrderResponseModel>();

            response = await orderService.AddOrder(requestModel);

            return Ok(response);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<OrderResponseModel>();

            response = await orderService.UpdateOrder(requestModel);

            return Ok(response);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? fruitId = null, [FromQuery] int? customerId = null)
        {
            var result = await orderService.GetOrders(page, pageSize, fruitId, customerId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var orders = await orderService.GetOrder(id);
            return Ok(orders);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var response = await orderService.DeleteOrder(id);

            return Ok(response);
        }
    }
}
