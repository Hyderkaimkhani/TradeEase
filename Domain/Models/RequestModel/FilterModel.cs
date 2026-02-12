namespace Domain.Models.RequestModel
{
    public class FilterModel
    {
        public int? EntityId { get; set; }      // CustomerId
        public int? AccountId { get; set; }     // e.g., AccountId for transactions
        public string? ReferenceNumber { get; set; } // e.g., SupplyNumber, OrderNumber, Account Number
        public int? FruitId { get; set; } // e.g., FruitId for Order and Supply
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDateUTC => FromDate?.Date;
        public DateTime? ToDateUTC => ToDate?.Date.AddDays(1).AddTicks(-1);
        public string? TransactionType { get; set; } // e.g., Payment, Expense, Income, Transfer, Adjustment
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
