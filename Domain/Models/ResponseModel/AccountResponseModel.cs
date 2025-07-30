using System.ComponentModel.DataAnnotations;

namespace Domain.Models.ResponseModel
{
    public class AccountResponseModel
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string? AccountNumber { get; set; }

        public string? BankName { get; set; }

        public decimal OpeningBalance { get; set; } = 0;

        public decimal CurrentBalance { get; set;} = 0;
    }
}
