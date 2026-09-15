using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
public class StoreResponseDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class StoreResponseModel
{
  public int StoreId { get; set; }
  public string? ClientId { get; set; }
  public string? StoreCode { get; set; }
  public string? StoreName { get; set; }
  public string? StoreCompany { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? StoreImage { get; set; }
  public string? LicenseNo { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; } 

  public AddressResponseDTO? Address { get; set; }
}
