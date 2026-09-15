using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.Models;
public class ClientResponseModel
{
  public string? ClientId { get;  set; }
  public string? ClientCode { get;  set; }
  public string? ClientName { get;  set; }
  public string? Username { get;  set; }
  public string? ClientImage { get;  set; }
  public int? CountryId { get;  set; }
  public int? AreaId { get;  set; }
  public int? CityId { get;  set; }
  public string? StreetAddress { get;  set; }
  public string? Zip { get;  set; }
  public string? ClientCompanyName { get;  set; }
  public string? Mobile { get;  set; }
  public string? Phone { get;  set; }
  public string? Email { get;  set; }
  public string? LicenseNo { get;  set; }
  public string? IdToken { get;  set; }
  public int? Ongfid { get;  set; }
  /// <summary>
  /// The Default value will get from Product Station Table
  /// </summary>
  public int? DefaultProductStationId { get;  set; }
  /// <summary>
  /// This column is used for order prefix 
  /// </summary>
  public int? ClientIdentifier { get;  set; }
  /// <summary>
  /// also vat no
  /// </summary>
  public string? Trnno { get;  set; }
  public string? StripeCustomerId { get;  set; }
  public int? DefaultStoreId { get;  set; }
  public int? DefaultCarrierId { get;  set; }
  public int? DefaultCurrencyId { get;  set; }
  public int? DefaultProductCategoryId { get;  set; } 
  public bool? Active { get;  set; }
  public bool? IsPaymentVerified { get;  set; }
  public bool? IsDefaultShipmentGrid { get;  set; }
  public int? RegionTimeZoneId { get; set; }
  public string? PublicKey { get; set; }
  public string? SecretKey { get; set; }
  public string? EncryptedKey { get; set; }
  public string? StripeWebhookSecret { get; set; }
  public string? StripeWebhookId { get; set; }
  public bool? AllowWithoutBalance { get; set; }
  public bool? AllowPersonalClientCarrierContract { get; set; }
  public bool? AllowShipperInvocie { get; set; }
  public bool? IsShowMetafield { get; set; }
}
