using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int ParentId { get; set; }
        public long? WialonUnitId { get; set; }
        public string WialonUniqueId { get; set; }
        public long? WialonUserId { get; set; }
        public string Token { get; set; }
      //  public string ProfilePicturePath { get; set; }
      //  public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int UserRoleId { get; set; }
        public DateTime? UserActivatedDate { get; set; }
        public int? TotalLogin { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; }

        public UserRoleModel UserRole { get; set; }

        public List<UnitModel> Units { get; set; }
    }
}
