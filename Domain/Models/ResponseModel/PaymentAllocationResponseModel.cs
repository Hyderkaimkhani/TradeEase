namespace Domain.Models.ResponseModel
{
    public class PaymentAllocationResponseModel
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string ReferenceType { get; set; } = string.Empty;// Order or Supply
        public int ReferenceId { get; set; }
        public decimal AllocatedAmount { get; set; }

        public string Number { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime OperationDate { get; set; } // Supply / Order Date
        public string TruckNumber { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
