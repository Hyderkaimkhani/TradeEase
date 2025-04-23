using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public int ParentId { get; set; }
        public string? Token { get; set; }        
        public string? ProfilePicturePath { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public int UserRoleId { get; set; }
        public DateTime? UserActivatedDate { get; set; }
        public int? TotalLogin { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual UserRole UserRole { get; set; }
    }
}
