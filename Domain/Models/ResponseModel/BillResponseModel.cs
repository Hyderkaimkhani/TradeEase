namespace Domain.Models.ResponseModel
{
    public class BillResponseModel
    {
        public int Id { get; set; }

        public string BillNumber { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty; // 'Customer' or 'Supplier'

        public int EntityId { get; set; }

        public string EntityName { get; set; } = string.Empty;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
        public string PaymentStatus { get; set; } = string.Empty; // 'Unpaid', 'Partial', 'Paid'

        public DateTime? PaidDate { get; set; }

        public string? PdfPath { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        public List<BillDetailRespnseModel> BillDetails { get; set; } = new List<BillDetailRespnseModel>();
    }

    public class BillDetailRespnseModel
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public string ReferenceType { get; set; } = string.Empty; // 'Order' or 'Supply'
        public int? OrderId { get; set; }
        public int? SupplyId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }    // Fruit, Truck, Notes, etc.
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? Unit { get; set; }
    }

}
