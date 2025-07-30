using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService AccountService;

        public AccountController(IAccountService AccountService)
        {
            this.AccountService = AccountService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] AccountAddModel model)
        {
            var result = await AccountService.AddAccount(model);
            return result.IsError ? BadRequest(result) : Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var result = await AccountService.GetAccount(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] FilterModel filterModel)
        {
            var result = await AccountService.GetAccounts(filterModel);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateModel model)
        {
            var result = await AccountService.UpdateAccount(model);
            return result.IsError ? NotFound(result) : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await AccountService.DeleteAccount(id);
            return result.IsError ? NotFound(result) : Ok(result);
        }
    }
}
