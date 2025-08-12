using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetOrders([FromQuery] FilterModel filter)
        {
            var result = await orderService.GetOrders(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await orderService.GetOrder(id);
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var response = await orderService.DeleteOrder(id);

            return Ok(response);
        }
    }
}
