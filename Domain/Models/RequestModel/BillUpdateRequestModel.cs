using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class BillUpdateRequestModel
    {
        [Required]
        public int Id { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }
    }
}
