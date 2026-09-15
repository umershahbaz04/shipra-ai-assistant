namespace Shipra.Backend.API.Core.Models;

public class ActiveCarrierPickupLocationForSelectionResponseModel
{
  public int ActiveCarrierPickupLocationId { get; set; }
  public int ActiveCarrierId { get; set; }
  public int CarrierId { get; set; }
  public int CountryId { get; set; }
  public string? StreetAddress { get; set; }
  public string? FullAddress { get; set; }
  public string? CarrierImage { get; set; }
  public string? CarrierName { get; set; }
  public string? LocationName { get; set; }
}
