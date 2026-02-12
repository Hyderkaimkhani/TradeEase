using System.ComponentModel.DataAnnotations;

namespace Domain.Models.ResponseModel
{
    public class AccountResponseModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public AccountResponseModel()
        {
            Name = string.Empty;
            Type = string.Empty;
        }
    }
}
