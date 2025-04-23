using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AppToken
    {
        public int Id { get; set; }
        
        public long ResourceId { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //public virtual User User { get; set; }
    }
}
