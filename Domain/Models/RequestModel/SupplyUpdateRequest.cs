using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.RequestModel
{
    public class SupplyUpdateRequest : SupplyAddRequest
    {
        public int Id { get; set; }
    }
}
