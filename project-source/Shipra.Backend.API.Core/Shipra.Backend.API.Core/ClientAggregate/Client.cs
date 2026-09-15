using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class Client
{
  public Client() { }
  public ClientId? ClientId { get; private set; }
  public string? ClientCode { get; private set; }
  public string? ClientName { get; private set; }
  public string? Username { get; private set; }
  public string? ClientImage { get; private set; }
  //public int? CountryId { get; private set; }
  //public int? RegionId { get; private set; }
  //public int? CityId { get; private set; }
  //public string? StreetAddress { get; private set; }
  //public string? Zip { get; private set; }
  public string? ClientCompanyName { get; private set; }
  public string? Mobile { get; private set; }
  public string? Phone { get; private set; }
  public string? Email { get; private set; }
  public string? LicenseNo { get; private set; }
  public string? IdToken { get; private set; }
  public int? Ongfid { get; private set; }
  /// <summary>
  /// The Default value will get from Product Station Table
  /// </summary>
  public int? DefaultProductStationId { get; private set; }
  /// <summary>
  /// This column is used for order prefix 
  /// </summary>
  public int? ClientIdentifier { get; private set; }
  /// <summary>
  /// also vat no
  /// </summary>
  public string? Trnno { get; private set; }
  public string? StripeCustomerId { get; private set; }
  public int? DefaultStoreId { get; private set; }
  public int? DefaultCarrierId { get; private set; }
  public int? DefaultCurrencyId { get; private set; }
  public int? DefaultProductCategoryId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? Active { get; private set; }
  public bool? IsPaymentVerified { get; private set; }
  public bool? IsDefaultShipmentGrid { get; private set; }
  public int? RegionTimeZoneId { get; set; }
  public string? PublicKey { get; set; }
  public string? SecretKey { get; set; }
  public string? StripeWebhookSecret { get; set; }
  public string? StripeWebhookId { get; set; }
  public string? EncryptedKey { get; set; }
  public int? PayoutScheduleSettings { get; set; }
  public decimal? PaymentProcessingCharges { get; set; }
  public bool? AllowWithoutBalance { get; set; }
  public bool? AllowPersonalClientCarrierContract { get; set; }
  public static Client CreateClient(ClientId clientId, string? clientName, string? clientImage, string? username, string? clientCompanyName, string? mobile, string? phone, string? email, EmployeeId? createdBy, int clientIdentifier, int OngfId, int regionTimeZoneId, string? publicKey, string? secretKey, string? encryptedKey)
  {
    var client = new Client()
    {
      ClientId = clientId,
      ClientCode = GetClientCode(clientIdentifier),
      ClientName = clientName,
      ClientImage = clientImage,
      Username = username,
      //CountryId = countryId,
      //RegionId = regionId,
      //CityId = cityId,
      ClientCompanyName = clientCompanyName,
      Mobile = UtilityHelper.CleanPhoneNumber(mobile),
      Phone = UtilityHelper.CleanPhoneNumber(phone),
      Email = email,
      ClientIdentifier = clientIdentifier,
      Ongfid = OngfId,
      //StreetAddress = streetAddress,
      //Zip = zip,
      DefaultCurrencyId = (int)EnumCurrency.AED,
      IsPaymentVerified = false,
      IsDefaultShipmentGrid = true,
      RegionTimeZoneId = regionTimeZoneId,
      PublicKey = publicKey,
      SecretKey = secretKey,
      EncryptedKey = encryptedKey,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,
      AllowPersonalClientCarrierContract = false,
      AllowWithoutBalance = false,
      Active = true
    };
    return client;
  }

  public static string GetClientCode(int clientIdentifier)
  {
    // 2-digit random number for uniqueness (10–99)
    var unique = Random.Shared.Next(10, 99); 
    // Format: CN + clientIdentifier + yyMM + unique
    return $"CN{clientIdentifier}{DateTime.Now:yyMM}{unique}";
  }

  public void UpdateClient(string? clientName, string? clientImage, string? clientCompanyName, string? mobile, string? phone, string? licenseNo, int regionTimeZoneId, EmployeeId? updatedBy)
  {
    ClientName = clientName;
    ClientImage = clientImage;
    ClientCompanyName = clientCompanyName;
    Mobile = UtilityHelper.CleanPhoneNumber(mobile);
    Phone = UtilityHelper.CleanPhoneNumber(phone);
    LicenseNo = licenseNo;
    RegionTimeZoneId = regionTimeZoneId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateClientProfileImage(string? clientImage, EmployeeId updatedBy)
  {
    ClientImage = clientImage;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void MarkVarifiedPayment(bool? isPaymentVerified, EmployeeId updatedBy)
  {
    IsPaymentVerified = isPaymentVerified;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateClientStripeInfo(string? stripeCustomerId, EmployeeId updatedBy)
  {
    StripeCustomerId = stripeCustomerId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientStripeWebhookSecret(string? stripeWebhookId, string? stripeWebhookSecret, EmployeeId updatedBy)
  {
    StripeWebhookSecret = stripeWebhookSecret;
    StripeWebhookId = stripeWebhookId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientDefaultStore(int? storeId, EmployeeId updatedBy)
  {
    DefaultStoreId = storeId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientDefaultProductStation(int? defaultStationId, EmployeeId? updatedBy)
  {
    DefaultProductStationId = defaultStationId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateClientDefaultCarrier(int? carrierId, EmployeeId updatedBy)
  {
    DefaultCarrierId = carrierId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientDefaultCurrency(int? currencyId, EmployeeId updatedBy)
  {
    DefaultCurrencyId = currencyId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientDefaultProductCategory(int? productCategoryId, EmployeeId updatedBy)
  {
    DefaultProductCategoryId = productCategoryId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateClientRegionTimeZone(int? regionTimeZone, EmployeeId updatedBy)
  {
    RegionTimeZoneId = regionTimeZone;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdatePaymentProcessingCharges(decimal? paymentProcessingCharges, int? payoutScheduleSettings)
  {
    PaymentProcessingCharges = paymentProcessingCharges;
    PayoutScheduleSettings = payoutScheduleSettings;
  }

  public void UpdateAllowPersonalClientCarrier(bool? isAllow)
  {
    AllowPersonalClientCarrierContract = isAllow;
  }
  public void UpdateAllowWithoutBalance(bool? isAllow)
  {
    AllowWithoutBalance = isAllow;
  }

  public void RotateKeys(string publicKey, string secretKey, string? encryptedValue)
  {
    PublicKey = publicKey;
    SecretKey = secretKey;
    EncryptedKey = encryptedValue;
  }
  public void UpdateEncryptedKey(string? encryptedKey)
  { 
    EncryptedKey = encryptedKey;
  }
}

public sealed record ClientId(Guid Value)
{
  public static ClientId New => new(Guid.NewGuid());
}
