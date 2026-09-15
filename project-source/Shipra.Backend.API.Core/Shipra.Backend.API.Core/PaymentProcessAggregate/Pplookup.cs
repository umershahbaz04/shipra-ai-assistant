using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.PaymentProcessAggregate;
public class Pplookup
{
  /// <summary>
  /// PaymentProcessId
  /// </summary>
  public int PplookupId { get; set; } 
  public string? Ppname { get; set; }
  public string? InputRequiredConfig { get; set; } 
}
