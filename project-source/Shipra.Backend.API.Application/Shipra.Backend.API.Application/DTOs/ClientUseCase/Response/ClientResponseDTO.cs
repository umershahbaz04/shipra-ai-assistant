using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;

public class ClientResponseModel
{
  public string? ClientId { get; set; }
  public string? ClientCode { get; set; }
  public string? ClientName { get; set; }
  public string? ClientImage { get; set; } 
  public string? ClientCompanyName { get; set; }
  public string? Mobile { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? LicenseNo { get; set; }
  public string? IdToken { get; set; }
  public int? Ongfid { get; set; }
  /// <summary>
  /// The Default value will get from Product Station Table
  /// </summary>
  public int? DefaultStationId { get; set; }
  /// <summary>
  /// This column is used for order prefix 
  /// </summary>
  public int? ClientIdentifier { get; set; }
  /// <summary>
  /// also vat no
  /// </summary>
  public string? Trnno { get; set; }
  public int? StoreId { get; set; }
  public bool Active { get; set; }
  public bool isPaymentVerified { get; set; }
  public AddressResponseDTO? Address { get; set; }
}
