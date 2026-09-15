using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Shipra.Backend.API.Application.Common;
public static class GuidHelper
{
  public static string GuidMessage { get; set; } = "Id should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)";
  public static bool Validator(string? input)
  {
    Guid guid;
    var hasGUID = Guid.TryParse(input!, out guid);
    return hasGUID;
  }
  public static Guid GetGuidFromString(string? input)
  { 
    return new Guid(input!);
  }
}
