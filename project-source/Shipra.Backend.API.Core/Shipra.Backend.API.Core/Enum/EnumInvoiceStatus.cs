using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Enum;
public enum EnumInvoiceStatus
{
  Draft = 1,            // Invoice created but not finalized
  Due = 2,              // Invoice issued, payment pending
  PartiallyPaid = 3,    // Partial payment received
  Paid = 4,             // Fully paid
  Overdue = 5,          // Past due date
  Cancelled = 6         // Cancelled / void
}
