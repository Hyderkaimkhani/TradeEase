using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class CustomerAddModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        //[MaxLength(50)]
        //public string PaymentTerms { get; set; }

        public decimal TotalCredit { get; set; } = 0;

        public decimal? CreditBalance { get; set; } = 0;
    }

}
