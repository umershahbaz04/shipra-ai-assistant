using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class BulkRegularPlaceorderRequestModel
{
  public string? CustomerName { get; set; }
  public string? MobileNumber { get; set; }
  public string? MobileNumber2 { get; set; }
  public string? Email { get; set; }
  public string? CountryCode { get; set; }
  public string? RegionName { get; set; }
  public string? CityCode { get; set; }
  public string? StreetAddress { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? PaymentMethod { get; set; }
  public decimal Amount { get; set; }
  public string? Remarks { get; set; }
  public string? Description { get; set; } 
}

public class BulkFullfilablePlaceorderRequestModel : BulkRegularPlaceorderRequestModel
{
  public string? Products { get; set; }
}
