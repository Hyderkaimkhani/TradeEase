# AccountTransaction System Integration Guide

## Overview
The AccountTransaction system provides a unified way to record all financial transactions in TradeEase. It replaces the need for separate Payment and Expense tables by using a single, flexible transaction table.

## Key Components Created

### 1. Entities
- `Account` - Company accounts (Cash, Bank, Receivables, Payables, etc.)
- `AccountTransaction` - All financial transactions

### 2. Services
- `IAccountService` / `AccountService` - Account management
- `IAccountTransactionService` / `AccountTransactionService` - Transaction management

### 3. Controllers
- `AccountController` - Account CRUD operations
- `AccountTransactionController` - Transaction operations and statements

### 4. Repository Interfaces & Implementations
- `IAccountRepository` / `AccountRepository`
- `IAccountTransactionRepository` / `AccountTransactionRepository`

## How to Use the AccountTransactionService in Other Services

### 1. Inject the Service
```csharp
public class OrderService : IOrderService
{
    private readonly IAccountTransactionService _accountTransactionService;
    
    public OrderService(
        // ... other dependencies
        IAccountTransactionService accountTransactionService)
    {
        _accountTransactionService = accountTransactionService;
    }
}
```

### 2. Record Transactions from Existing Services

#### In OrderService.AddOrder():
```csharp
// After successfully creating the order
var orderAmount = requestModel.Quantity * requestModel.Price;
await _accountTransactionService.RecordOrderTransactionAsync(
    orderId: order.Id,
    customerId: requestModel.CustomerId,
    amount: orderAmount,
    companyId: currentUser.CompanyId,
    createdBy: currentUser.UserName
);
```

#### In SupplyService.AddSupply():
```csharp
// After successfully creating the supply
var supplyAmount = requestModel.Quantity * requestModel.Price;
await _accountTransactionService.RecordSupplyTransactionAsync(
    supplyId: supply.Id,
    supplierId: requestModel.SupplierId,
    amount: supplyAmount,
    companyId: currentUser.CompanyId,
    createdBy: currentUser.UserName
);
```

#### In PaymentService.AddPayment():
```csharp
// After successfully creating the payment
await _accountTransactionService.RecordPaymentTransactionAsync(
    paymentId: payment.Id,
    entityId: requestModel.EntityId,
    amount: requestModel.Amount,
    paymentMethod: requestModel.PaymentMethod,
    companyId: currentUser.CompanyId,
    createdBy: currentUser.UserName
);
```

### 3. Direct Expense/Income Recording
```csharp
// Record an expense
await _accountTransactionService.RecordExpenseTransactionAsync(
    category: "Rent",
    party: "Landlord",
    amount: 1000,
    accountId: cashAccountId,
    companyId: currentUser.CompanyId,
    createdBy: currentUser.UserName
);

// Record income
await _accountTransactionService.RecordIncomeTransactionAsync(
    category: "Interest",
    party: "Bank",
    amount: 200,
    accountId: bankAccountId,
    companyId: currentUser.CompanyId,
    createdBy: currentUser.UserName
);
```

## API Endpoints

### Account Management
```
POST   /api/Account                    - Add new account
PUT    /api/Account/{id}              - Update account
GET    /api/Account/{id}              - Get account by ID
GET    /api/Account                   - Get all accounts
GET    /api/Account/type/{type}       - Get accounts by type
DELETE /api/Account/{id}              - Delete account
```

### Transaction Management
```
POST   /api/AccountTransaction                    - Add transaction
GET    /api/AccountTransaction/{id}              - Get transaction by ID
GET    /api/AccountTransaction                   - Get transactions with filters
POST   /api/AccountTransaction/statement         - Get account statement
DELETE /api/AccountTransaction/{id}              - Delete transaction
POST   /api/AccountTransaction/expense           - Add expense directly
POST   /api/AccountTransaction/income            - Add income directly
```

## Account Statement Generation

### Request Model
```json
{
  "accountId": 1,
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31",
  "entityId": 5,           // Optional: Filter by customer/supplier
  "transactionType": "Payment"  // Optional: Filter by transaction type
}
```

### Response
```json
{
  "accountId": 1,
  "accountName": "Cash",
  "accountType": "Cash",
  "fromDate": "2024-01-01",
  "toDate": "2024-12-31",
  "openingBalance": 1000.00,
  "closingBalance": 2500.00,
  "transactions": [
    {
      "id": 1,
      "transactionType": "Order",
      "transactionDirection": "Debit",
      "amount": 500.00,
      "signedAmount": 500.00,
      "transactionDate": "2024-01-15",
      "entityName": "Customer A",
      "referenceType": "Order",
      "referenceId": 101,
      "notes": "Order #101"
    }
  ]
}
```

## Default Account Types

The system automatically creates default accounts if they don't exist:

- **Receivables** - For customer orders (money owed to you)
- **Payables** - For supplier supplies (money you owe)
- **Cash** - For cash transactions
- **Bank** - For bank transactions

## Transaction Types and Directions

| Transaction Type | Direction | Description |
|------------------|-----------|-------------|
| Order | Debit | Customer buys from you (money coming IN) |
| Supply | Credit | Supplier supplies to you (money going OUT) |
| Payment | Credit | You pay someone (money going OUT) |
| Expense | Credit | Company expense (money going OUT) |
| Income | Debit | Company income (money coming IN) |
| Transfer | Both | Money transfer between accounts |

## Integration Steps

### 1. Register Services in Program.cs
```csharp
// Add to your service registration
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountTransactionService, AccountTransactionService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountTransactionRepository, AccountTransactionRepository>();
```

### 2. Update Existing Services
- Inject `IAccountTransactionService` into your existing services
- Call the appropriate recording methods after successful operations
- Remove direct payment/expense logic if it exists

### 3. Update Database
- Run the Account and AccountTransaction table creation scripts
- Migrate existing payment data if needed

### 4. Test Integration
- Test account creation and management
- Test transaction recording from existing services
- Test account statement generation
- Test expense/income recording

## Benefits

1. **Unified Ledger** - All financial transactions in one place
2. **Audit Trail** - Complete transaction history
3. **Flexible** - Supports all types of financial events
4. **Scalable** - Easy to add new transaction types
5. **Reporting** - Rich reporting capabilities
6. **Multi-tenant** - Company-level data isolation

## Example Usage in Existing Services

### OrderService Integration
```csharp
public async Task<ResponseModel<OrderResponseModel>> AddOrder(OrderAddModel requestModel)
{
    // ... existing order creation logic ...
    
    if (await unitOfWork.SaveChangesAsync())
    {
        // Record the transaction
        var orderAmount = requestModel.Quantity * requestModel.Price;
        await _accountTransactionService.RecordOrderTransactionAsync(
            order.Id, 
            requestModel.CustomerId, 
            orderAmount, 
            currentUser.CompanyId, 
            currentUser.UserName
        );
        
        // ... rest of the method
    }
}
```

This integration ensures that every order automatically creates a corresponding transaction record, maintaining accurate financial records without manual intervention. 