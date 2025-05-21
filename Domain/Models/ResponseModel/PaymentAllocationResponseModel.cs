namespace Domain.Models.ResponseModel
{
    public class PaymentAllocationResponseModel
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string ReferenceType { get; set; } // Order or Supply
        public int ReferenceId { get; set; }
        public decimal AllocatedAmount { get; set; }
        public PaymentResponseModel Payment { get; set; }
    }
}
