using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Obsolete]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        #region Client

        [Authorize]
        [HttpPost("Client")]
        public async Task<IActionResult> AddClient(ClientModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<ClientModel>();
            if (requestModel.Id == 0)
            {
                response = await _adminService.AddClient(requestModel);
            }
            else
            {
                response = await _adminService.UpdateClient(requestModel);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Client")]
        public async Task<IActionResult> GetClients()
        {
            var response = await _adminService.GetAllClients();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Client/{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var response = await _adminService.GetClient(id);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Client/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var response = await _adminService.DeleteClient(id);
            return Ok(response);
        }

        #endregion

        #region Consumer
        [Authorize]
        [HttpPost("Consumer")]
        public async Task<IActionResult> AddConsumer(ConsumerModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<ConsumerModel>();
            if (requestModel.Id == 0)
            {
                response = await _adminService.AddConsumer(requestModel);
            }
            else
            {
                response = await _adminService.UpdateConsumer(requestModel);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Consumer")]
        public async Task<IActionResult> GetConsumers()
        {
            var response = await _adminService.GetAllConsumers();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Consumer/{id}")]
        public async Task<IActionResult> GetConsumer(int id)
        {
            var response = await _adminService.GetConsumer(id);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Consumer/{id}")]
        public async Task<IActionResult> DeleteConsumer(int id)
        {
            var response = await _adminService.DeleteConsumer(id);
            return Ok(response);
        }
        #endregion

        #region Customer

        [Authorize]
        [HttpPost("Customer")]
        public async Task<IActionResult> AddCustomer(CustomerModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = new ResponseModel<CustomerModel>();
            if (requestModel.Id == 0)
            {
                response = await _adminService.AddCustomer(requestModel);
            }
            else
            {
                response = await _adminService.UpdateCustomer(requestModel);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Customer")]
        public async Task<IActionResult> GetCustomers()
        {
            var response = await _adminService.GetAllCustomers();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Customer/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var response = await _adminService.GetCustomer(id);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Customer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var response = await _adminService.DeleteCustomer(id);
            return Ok(response);
        }

        #endregion
    }
}