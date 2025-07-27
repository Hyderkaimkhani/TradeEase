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
        Credit,     // Incoming
        Debit       // Outgoing
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
}
