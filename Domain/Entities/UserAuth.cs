using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class UserAuth
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public string JWTToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime RefreshTokenValidTill { get; set; }

        public virtual User User { get; set; }
    }
}
