using System.ComponentModel.DataAnnotations;

namespace Domain.Models.RequestModel
{
    public class FruitAddModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Kg|Dozen|Box|Man)$", ErrorMessage = "Invalid UnitType")]
        public string UnitType { get; set; } = "Man";

        public bool isActive { get; set; } = true;

    }
}
