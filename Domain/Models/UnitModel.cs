using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class UnitModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long? WialonUnitId { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string RegistrationNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
