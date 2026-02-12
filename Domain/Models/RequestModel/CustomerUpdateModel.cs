using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class CustomerUpdateModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string PaymentTerms { get; set; } = string.Empty;

        //public decimal TotalCredit { get; set; } = 0;

        public decimal? CreditBalance { get; set; } = 0;
    }
}
