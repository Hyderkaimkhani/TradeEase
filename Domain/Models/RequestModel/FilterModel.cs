namespace Domain.Models.RequestModel
{
    public class FilterModel
    {
        public int? EntityId { get; set; }
        public string? ReferenceNumber { get; set; } // e.g., SupplyNumber, OrderNumber, Account Number
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
