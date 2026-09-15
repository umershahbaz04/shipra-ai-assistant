using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.DTOs.Common.Request;
public class CommonFilterModel  
{
  public FilterModelDTO? FilterModel { get; set; } 
}
public class CommonAddressFilterModel : CommonFilterModel
{
  public string? CountryId { get; set; } = string.Empty;
  public Dictionary<string, AddressFilterModel>? OrderAddressFilter { get; set; } = new();
}
