using Common;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api")]
    [Authorize]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Customer

        [HttpPost("Customer")]
        public async Task<IActionResult> AddCustomer(CustomerAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _adminService.AddCustomer(requestModel);

            return Ok(response);
        }

        [HttpPost("Customer/Update")]
        public async Task<IActionResult> UpdateCustomer(CustomerUpdateModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _adminService.UpdateCustomer(requestModel);

            return Ok(response);
        }

        [HttpGet("Customer")]
        public async Task<IActionResult> GetCustomers([FromQuery] bool? isActive, [FromQuery] string? entityType)
        {
            if (!string.IsNullOrEmpty(entityType))
            {
                if (!Enum.TryParse<EntityType>(entityType, true, out _))
                {
                    var validTypes = string.Join(", ", Enum.GetNames(typeof(EntityType)));
                    return BadRequest($"Invalid EntityType. Valid values are: {validTypes}");
                }
            }

            return Ok(await _adminService.GetCustomers(isActive, entityType));
        }

        [HttpGet("Customer/dropdown")]
        public async Task<IActionResult> GetCustomersDropDown()
        {
            return Ok(await _adminService.GetCustomersDropDown(EntityType.Customer.ToString()));

        }

        [HttpGet("Supplier/dropdown")]
        public async Task<IActionResult> GetSuppliers()
        {
            return Ok(await _adminService.GetCustomersDropDown(EntityType.Customer.ToString()));

        }

        [HttpGet("Customer/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var response = await _adminService.GetCustomer(id);
            return Ok(response);
        }

        [HttpDelete("Customer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var response = await _adminService.DeleteCustomer(id);
            return Ok(response);
        }

        #endregion

        #region Fruit

        [HttpPost("Fruit")]
        public async Task<IActionResult> AddFruit(FruitAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<FruitResponseModel>();
            if (requestModel.Id == 0)
            {
                response = await _adminService.AddFruit(requestModel);
            }
            else
            {
                response = await _adminService.UpdateFruit(requestModel);
            }
            return Ok(response);
        }

        [HttpGet("Fruit")]
        public async Task<IActionResult> GetFruits()
        {
            return Ok(await _adminService.GetFruits());

        }


        [HttpGet("Fruit/{id}")]
        public async Task<IActionResult> GetFruit(int id)
        {
            var response = await _adminService.GetFruit(id);
            return Ok(response);
        }

        #endregion
    }
}