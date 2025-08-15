namespace Common
{
    public class Enums
    {
    }

    public enum EntityType
    {
        Customer,
        Supplier,
    }

    public enum OperationType
    {
        Supply,
        Order,
    }

    public enum PaymentStatus
    {
        Unpaid,
        Partial,
        Paid,
    }
    public enum OrderStatus
    {
        Pending,
        Dispatched,
        Delivered,
        Canceled
    }

    public enum TransactionDirection
    {
        Credit,     // Outgoing
        Debit       // Incoming
    }

    public enum TransactionType
    {
        Payment,
        Expense,
        Income,
        Transfer,
        Adjustment,
        Order,
        Supply,
    }

    public enum AccountType
    {
        Recievable,
        Payable,
        Cash,
        Bank,
        MobilePayment,
        CreditCard,
        Other
    }

    public enum ReferenceType
    {
        Order, 
        Supply, 
        Expense, 
        Payment, 
        Transfer, 
        OpeningBalance
    }

    public enum TransactionFlow
    {
        Paid,     // Outgoing
        Received       // Incoming
    }
}
