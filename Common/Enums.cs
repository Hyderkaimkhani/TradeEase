using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Enums
    {
    }

    public enum EntityType
    {
        Customer,
        Supplier,
    }

    public enum OperationType
    {
        Supply,
        Order,
    }

    public enum PaymentStatus
    {
        Unpaid,
        Partial,
        Paid,
    }
    public enum OrderStatus
    {
        Pending,
        Dispatched,
        Delivered,
        Canceled
    }
}
