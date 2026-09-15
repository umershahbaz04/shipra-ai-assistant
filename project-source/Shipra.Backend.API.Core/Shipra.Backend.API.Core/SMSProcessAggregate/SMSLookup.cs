using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.SMSProcessAggregate;
public class SMSLookup
{
  /// <summary>
  /// SMSLookup
  /// </summary>
  public int SMSLookupId { get; set; }
  public string? ServiceName { get; set; }
  public string? InputRequiredConfig { get; set; }
  public string? Config { get; set; }

  public static SMSLookup AddDefault()
  {
    return new SMSLookup()
    {
      SMSLookupId = 0,
      ServiceName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
