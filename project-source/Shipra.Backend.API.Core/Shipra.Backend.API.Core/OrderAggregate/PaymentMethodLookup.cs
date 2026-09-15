using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class PaymentMethodLookup
{
  public int PaymentMethodId { get; set; }
  public string? Code { get; set; }
  public string? PMName { get; set; }

  public static PaymentMethodLookup AddDefault()
  {
    return new PaymentMethodLookup()
    {
      PaymentMethodId = 0,
      PMName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
