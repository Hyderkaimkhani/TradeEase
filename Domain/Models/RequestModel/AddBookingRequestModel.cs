using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Models.RequestModel
{
    public class AddBookingRequestModel
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        public List<int> Services { get; set; }
    }
}
