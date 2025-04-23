using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; }
        public int AccessTokenExpiry { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
