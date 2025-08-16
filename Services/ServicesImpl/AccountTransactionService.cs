using AutoMapper;
using Common;
using Common.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models.ResponseModel;
using Repositories.Interfaces;
using Repositories.RepositoriesImpl;
using Services.Interfaces;
using System.ComponentModel.Design;

namespace Services.ServicesImpl
{
    public class AccountTransactionService : IAccountTransactionService
    {
        private readonly IMapper autoMapper;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ICurrentUserService _currentUserService;

        public AccountTransactionService(IUnitOfWorkFactory unitOfWorkFactory, ICurrentUserService currentUserService,
        IMapper mapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            autoMapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseModel<AccountTransactionResponseModel>> AddTransaction(AccountTransactionAddModel requestModel)
        {
            var response = new ResponseModel<AccountTransactionResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                // Validate account exists
                var account = await unitOfWork.AccountRepository.GetAccount(requestModel.AccountId);
                if (account == null)
                {
                    response.IsError = true;
                    response.Message = "Account not found";
                    return response;
                }

                var transaction = autoMapper.Map<AccountTransaction>(requestModel);

                var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                // Update account balance
                UpdateAccountBalance(account, transaction);
                if (await unitOfWork.SaveChangesAsync())
                {
                    var transactionResponse = autoMapper.Map<AccountTransactionResponseModel>(addedTransaction);
                    response.Model = transactionResponse;
                    response.Message = "Transaction added successfully";
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Failed to save transaction";

                }
            }
            return response;
        }

        
        public async Task<ResponseModel<AccountTransactionResponseModel>> GetTransaction(int id)
        {
            var response = new ResponseModel<AccountTransactionResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var transaction = await unitOfWork.AccountTransactionRepository.GetTransactionDetail(id);
                if (transaction == null)
                {
                    response.IsError = true;
                    response.Message = "Transaction not found";
                    return response;
                }

                var transactionResponse = autoMapper.Map<AccountTransactionResponseModel>(transaction);
                response.Model = transactionResponse;
            }

            return response;
        }

        public async Task<PaginatedResponseModel<AccountTransactionResponseModel>> GetTransactions(FilterModel filterModel)
        {
            var response = new PaginatedResponseModel<AccountTransactionResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {

                var transactions = await unitOfWork.AccountTransactionRepository.GetTransactions(filterModel);
                if (transactions.TotalCount > 0)
                {
                    response.Model = autoMapper.Map<List<AccountTransactionResponseModel>>(transactions.Model);
                    response.TotalCount = transactions.TotalCount;
                }
                else
                {
                    response.Model = new List<AccountTransactionResponseModel>();
                    response.Message = "No transactions found";
                }
            }
            return response;
        }

        public async Task<ResponseModel<AccountStatementResponseModel>> GetAccountStatement(AccountStatementRequestModel requestModel)
        {
            var response = new ResponseModel<AccountStatementResponseModel>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                requestModel.CompanyId = _currentUserService.GetCurrentCompanyId();
                var statement = await unitOfWork.AccountTransactionRepository.GetAccountStatement(requestModel);
                response.Model = statement;
            }
            return response;
        }

        public async Task<ResponseModel<bool>> DeleteTransaction(int id)
        {
            var response = new ResponseModel<bool>();

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var transaction = await unitOfWork.AccountTransactionRepository.GetTransaction(id);
                if (transaction == null)
                {
                    response.IsError = true;
                    response.Message = "Transaction not found";
                    return response;
                }

                // Soft delete
                transaction.IsActive = false;

                await ReverseAccountBalance(unitOfWork, transaction);
                if (await unitOfWork.SaveChangesAsync())
                {
                    response.Model = true;
                    response.Message = "Transaction deleted successfully";
                }
                else
                {
                    response.IsError = true;
                    response.Message = "Unable to delete Transaction";
                    response.Model = false;
                }
            }
            return response;
        }

        // Service methods to be called from other services

        public async Task<ResponseModel<bool>> RecordOrderTransaction(int orderId, int customerId, decimal amount, DateTime transactionDate)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                // Get default receivables account
                var receivableAccount = await unitOfWork.AccountRepository.GetAccountReceivable();
                if (receivableAccount == null)
                {
                    response.IsError = true;
                    response.Message = "Please create Receivable account";
                    return response;
                }
                else
                {
                    var transaction = new AccountTransaction
                    {
                        AccountId = receivableAccount.Id,
                        TransactionType = TransactionType.Order.ToString(),
                        TransactionDirection = TransactionDirection.Debit.ToString(), // Money coming IN (customer owes you)
                        Amount = amount,
                        TransactionDate = transactionDate,
                        EntityId = customerId,
                        ReferenceType = OperationType.Order.ToString(),
                        ReferenceId = orderId,
                        Notes = "Sales to Customer",
                    };

                    var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                    // Update account balance
                    UpdateAccountBalance(receivableAccount, transaction);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Order transaction recorded successfully";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Failed to record order transaction";
                        response.Model = false;
                    }
                }
                return response;
            }
        }

        public async Task<ResponseModel<bool>> RecordSupplyTransaction(int supplyId, int supplierId, decimal amount, DateTime transactionDate)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                // Get default payables account
                var payablesAccount = await unitOfWork.AccountRepository.GetAccountPayable();
                if (payablesAccount == null)
                {
                    response.IsError = true;
                    response.Message = "Please create Payable account";
                    return response;
                }
                else
                {
                    var transaction = new AccountTransaction
                    {
                        AccountId = payablesAccount.Id,
                        TransactionType = TransactionType.Supply.ToString(),
                        TransactionDirection = TransactionDirection.Credit.ToString(), // Money going OUT (you owe supplier)
                        Amount = amount,
                        TransactionDate = transactionDate,
                        EntityId = supplierId,
                        ReferenceType = OperationType.Supply.ToString(),
                        ReferenceId = supplyId,
                        Notes = "Purchase from Supplier",
                    };

                    var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                    // Update account balance
                    var account = payablesAccount;
                    UpdateAccountBalance(account, transaction);

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Supply transaction recorded successfully";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Failed to record supply transaction";
                        response.Model = false;
                    }
                }
            }
            return response;
        }

        public async Task<ResponseModel<int>> RecordPaymentTransaction(PaymentAddModel paymentModel)
        {
            var response = new ResponseModel<int>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                // Get 
                var account = await unitOfWork.AccountRepository.GetAccount(paymentModel.AccountId);
                if (account != null)
                {
                    var transaction = new AccountTransaction
                    {
                        AccountId = paymentModel.AccountId,
                        TransactionType = TransactionType.Payment.ToString(),
                        //TransactionDirection = paymentModel.TransactionDirection,
                        Amount = paymentModel.Amount,
                        TransactionDate = paymentModel.PaymentDate,
                        PaymentMethod = paymentModel.PaymentMethod,
                        EntityId = paymentModel.EntityId,
                        ReferenceType = ReferenceType.Payment.ToString(),
                        Notes = paymentModel.Notes
                    };

                    var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                    UpdateAccountBalance(account, transaction);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = addedTransaction.Id;
                        response.Message = "Payment transaction recorded successfully";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Failed to record Payment transaction";
                        response.Model = 0;
                    }

                }
            }
            return response;
        }

        public async Task<ResponseModel<bool>> RecordExpenseTransaction(string category, string party, decimal amount, int accountId)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var account = await unitOfWork.AccountRepository.GetAccount(accountId);
                if (account == null)
                {
                    response.IsError = true;
                    response.Message = "Account doesn't exists.";
                    return response;
                }
                else
                {
                    var transaction = new AccountTransaction
                    {
                        AccountId = accountId,
                        TransactionType = TransactionType.Expense.ToString(),
                        TransactionDirection = "Credit", // Money going OUT
                        Amount = amount,
                        TransactionDate = DateTime.Now,
                        Category = category,
                        Party = party,
                        ReferenceType = "Expense",
                        ReferenceId = 0, // No specific reference for general expenses
                    };

                    var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                    // Update account balance
                    //var account = await unitOfWork.AccountRepository.GetAccountByIdAsync(accountId, companyId);
                    UpdateAccountBalance(account, transaction);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Expense transaction recorded successfully";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Failed to record Expense transaction";
                        response.Model = false;
                    }
                }

            }
            return response;
        }

        public async Task<ResponseModel<bool>> RecordIncomeTransaction(string category, string party, decimal amount, int accountId)
        {
            var response = new ResponseModel<bool>();
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                var account = await unitOfWork.AccountRepository.GetAccount(accountId);
                if (account == null)
                {
                    response.IsError = true;
                    response.Message = "Account doesn't exists.";
                    return response;
                }
                else
                {
                    var transaction = new AccountTransaction
                    {
                        AccountId = accountId,
                        TransactionType = TransactionType.Income.ToString(),
                        TransactionDirection = "Debit", // Money coming IN
                        Amount = amount,
                        TransactionDate = DateTime.Now,
                        Category = category,
                        Party = party,
                        ReferenceType = "Income",
                        ReferenceId = 0, // No specific reference for general income
                    };

                    var addedTransaction = await unitOfWork.AccountTransactionRepository.AddTransaction(transaction);

                    // Update account balance
                    UpdateAccountBalance(account, transaction);
                    if (await unitOfWork.SaveChangesAsync())
                    {
                        response.Model = true;
                        response.Message = "Income transaction recorded successfully";
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Failed to record Income transaction";
                        response.Model = false;
                    }
                }
            }
            return response;
        }

        public void UpdateAccountBalance(Account account, AccountTransaction transaction)
        {

            if (transaction.TransactionDirection == "Debit")
            {
                account.CurrentBalance += transaction.Amount;
            }
            else
            {
                account.CurrentBalance -= transaction.Amount;
            }

            account.UpdatedDate = DateTime.Now;
        }

        private async Task ReverseAccountBalance(IUnitOfWork unitOfWork, AccountTransaction transaction)
        {
            var account = await unitOfWork.AccountRepository.GetAccount(transaction.AccountId);
            if (account != null)
            {
                if (transaction.TransactionDirection == "Debit")
                {
                    account.CurrentBalance -= transaction.Amount;
                }
                else
                {
                    account.CurrentBalance += transaction.Amount;
                }
            }
        }
    }
}