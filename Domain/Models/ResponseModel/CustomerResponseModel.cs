namespace Domain.Models.ResponseModel
{
    public class CustomerResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string? Address { get; set; }

        public string? PaymentTerms { get; set; }

        public decimal TotalCredit { get; set; }

        public decimal? CreditBalance { get; set; }

        public bool IsActive { get; set; }

        public CustomerResponseModel()
        {
            Name = string.Empty;
            Phone = string.Empty;
            City = string.Empty;
            Address = string.Empty;
        }
    }

}
