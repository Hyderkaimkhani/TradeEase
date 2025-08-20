using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountTransactionController : ControllerBase
    {
        private readonly IAccountTransactionService _accountTransactionService;

        public AccountTransactionController(IAccountTransactionService accountTransactionService)
        {
            _accountTransactionService = accountTransactionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] AccountTransactionAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            if (requestModel.TransactionType == "Transfer")
            {
                var response = await _accountTransactionService.RecordTransferTransaction(requestModel);
                return Ok(response);
            }
            else
            {
                var response = await _accountTransactionService.AddTransaction(requestModel);
                return Ok(response);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var response = await _accountTransactionService.GetTransaction(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] FilterModel filterModel)
        {
            var response = await _accountTransactionService.GetTransactions(filterModel);
            return Ok(response);
        }

        [HttpPost("statement")]
        public async Task<IActionResult> GetAccountStatement([FromBody] AccountStatementRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _accountTransactionService.GetAccountStatement(requestModel);
            return Ok(response);
        }

        [HttpPost("export-statement")]
        public async Task<IActionResult> ExportStatement([FromBody] AccountStatementRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _accountTransactionService.GetAccountStatement(requestModel);
            var pdf = new StatementDocument(response.Model).GeneratePdf();

            return File(pdf, "application/pdf", "Statement.pdf");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var response = await _accountTransactionService.DeleteTransaction(id);
            return Ok(response);
        }
    }
}