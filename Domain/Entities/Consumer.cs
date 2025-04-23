using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class Consumer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DeviceId { get; set; }
        public int CustomerId { get; set; }
        public int SimId { get; set; }
        public string City { get; set; }
        public int InitialMileage { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
