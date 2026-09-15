using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.WalletAggregate;

public  class PaymentLinkStatusLookup
{
  public int PaymentLinkStatusId { get; set; } 
  public string? StatusName { get; set; }
   
  public static PaymentLinkStatusLookup AddDefault()
  {
    return new PaymentLinkStatusLookup()
    {
      PaymentLinkStatusId = 0,
      StatusName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
