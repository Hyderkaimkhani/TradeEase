namespace Domain.Models.ResponseModel
{
    public class UnallocatedTransaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
