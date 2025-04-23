using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string PaymentTerms { get; set; } = "Cash";

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCredit { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CreditBalance { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }

}
