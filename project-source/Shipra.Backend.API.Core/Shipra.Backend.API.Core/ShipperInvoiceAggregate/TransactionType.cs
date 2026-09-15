using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;
public class TransactionType
{
  public int TransactionTypeId { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
}
