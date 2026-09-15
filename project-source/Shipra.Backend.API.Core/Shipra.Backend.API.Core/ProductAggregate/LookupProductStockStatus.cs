using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class LookupProductStockStatus
{
  public int ProductStockStatusId { get; set; } 
  public string? StatusName { get; set; }
}
