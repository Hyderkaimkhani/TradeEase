using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BillDetail
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Bill")]
        public int BillId { get; set; }

        [Required]
        [StringLength(20)]
        public string ReferenceType { get; set; } = string.Empty; // 'Order' or 'Supply'

        [ForeignKey("Order")]
        public int? OrderId { get; set; }

        [ForeignKey("Supply")]
        public int? SupplyId { get; set; }

        [StringLength(20)]
        public string? ReferenceNumber { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        public Bill Bill { get; set; } = new Bill();
        public virtual Order? Order { get; set; }
        public virtual Supply? Supply { get; set; }
    }

}
