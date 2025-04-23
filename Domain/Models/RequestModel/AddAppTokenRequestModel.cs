using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Models.RequestModel
{
    public class AppTokenRequestModel
    {
        [Required]
        public long ResourceId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
