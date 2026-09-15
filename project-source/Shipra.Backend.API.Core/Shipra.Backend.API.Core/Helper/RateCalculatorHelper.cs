using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Helper;
public static class RateCalculatorHelper
{ 
  public static decimal RoundToNearestHalf(decimal value)
  {
    // Example: 1.2 -> 1.5, 1.6 -> 2
    return Math.Ceiling(value * 2) / 2;
  }
}
