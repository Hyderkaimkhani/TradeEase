using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService AccountService)
        {
            this.accountService = AccountService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAccount([FromBody] AccountAddModel model)
        {
            var result = await accountService.AddAccount(model);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var result = await accountService.GetAccount(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] FilterModel filterModel)
        {
            var result = await accountService.GetAccounts(filterModel);
            return Ok(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdateModel model)
        {
            var result = await accountService.UpdateAccount(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await accountService.DeleteAccount(id);
            return Ok(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetAccounts()
        {
            return Ok(await accountService.GetAccountsDropDown());

        }
    }
}
