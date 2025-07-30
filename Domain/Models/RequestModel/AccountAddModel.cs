using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class AccountAddModel
    {

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(20)]
        [RegularExpression("^(Cash|Bank|Wallet|Savings|Loan|Investment|Others)$", ErrorMessage = "Invalid Account Type")]
        public string Type { get; set; } = null!; // Cash, Bank, Wallet, Other

        [MaxLength(50)]
        public string? AccountNumber { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        public decimal OpeningBalance { get; set; } = 0;
    }
}
