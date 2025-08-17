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

            var response = await _accountTransactionService.AddTransaction(requestModel);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var response = await _accountTransactionService.GetTransaction(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(FilterModel filterModel)
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

        [HttpGet("export-statement")]
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

        // Direct expense/income endpoints
        [HttpPost("expense")]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var transactionModel = new AccountTransactionAddModel
            {
                AccountId = requestModel.AccountId,
                TransactionType = "Expense",
                TransactionDirection = "Credit",
                Amount = requestModel.Amount,
                TransactionDate = requestModel.ExpenseDate,
                PaymentMethod = requestModel.PaymentMethod,
                Notes = requestModel.Description,
                EntityId = requestModel.EntityId,
                ReferenceType = "Expense",
                ReferenceId = 0, // Will be set by service
                Category = requestModel.ExpenseType,
                Party = requestModel.PaidTo
            };

            var response = await _accountTransactionService.AddTransaction(transactionModel);
            return Ok(response);
        }

        [HttpPost("income")]
        public async Task<IActionResult> AddIncome([FromBody] IncomeAddModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var transactionModel = new AccountTransactionAddModel
            {
                AccountId = requestModel.AccountId,
                TransactionType = "Income",
                TransactionDirection = "Debit",
                Amount = requestModel.Amount,
                TransactionDate = requestModel.IncomeDate,
                PaymentMethod = requestModel.PaymentMethod,
                Notes = requestModel.Description,
                EntityId = requestModel.EntityId,
                ReferenceType = "Income",
                ReferenceId = 0, // Will be set by service
                Category = requestModel.IncomeType,
                Party = requestModel.ReceivedFrom
            };

            var response = await _accountTransactionService.AddTransaction(transactionModel);
            return Ok(response);
        }
    }

    // Helper models for direct expense/income endpoints
    public class ExpenseAddModel
    {
        public int AccountId { get; set; }
        public string ExpenseType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; } = DateTime.Now;
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public string? PaidTo { get; set; }
        public int? EntityId { get; set; }
    }

    public class IncomeAddModel
    {
        public int AccountId { get; set; }
        public string IncomeType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IncomeDate { get; set; } = DateTime.Now;
        public string? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public string? ReceivedFrom { get; set; }
        public int? EntityId { get; set; }
    }
}