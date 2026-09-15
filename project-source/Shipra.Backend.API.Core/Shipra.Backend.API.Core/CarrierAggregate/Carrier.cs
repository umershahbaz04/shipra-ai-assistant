using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class Carrier
{
  public int CarrierId { get; set; }
  public string? Name { get; set; }
  public string? CarrierImage { get; set; }
  public string? CarrierWebsite { get; set; }
  public bool? IsClientCarrier { get; set; }
  public bool? IsDispatchExCompany { get; set; }
  public string? Config { get; set; }
  public string? InputRequiredConfig { get; set; }
  public string? SettingConfig { get; set; }
  public string? GuideUrl { get; set; }
  public int? RegionTimeZoneId { get; set; }
  public bool? IsWebhookSupported { get; set; }
  public string? BackgroundColor { get; set; }
  public string? BorderColor { get; set; }
  public int? CountryId { get; set; }
  public int? DisplayOrder { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; }
  public bool? IsRateCheck { get; set; }
  public bool? ValidateAddress { get; set; }

  public static Carrier CreateCarrier(int carrierId, string? name, string? carrierImage, string? carrierWebsite, string? config, string? inputRequiredConfig, int? countryId, string? settingConfig, bool? isRateCheck, bool? validateAddress, EmployeeId createBy)
  {
    return new Carrier()
    {
      CarrierId = carrierId,
      Name = name,
      CarrierImage = carrierImage,
      CarrierWebsite = carrierWebsite,
      SettingConfig = settingConfig,
      Config = config,
      InputRequiredConfig = inputRequiredConfig,
      IsClientCarrier = false,
      CountryId = countryId,
      Active = true,
      IsDefault = false,
      IsRateCheck = isRateCheck,
      ValidateAddress = validateAddress,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createBy
    };
  }

  public void UpdateCarrier(string? name, string? carrierImage, string? carrierWebsite, string? config, string? inputRequiredConfig, int? countryId, EmployeeId updatedBy)
  {
    Name = name;
    CarrierImage = carrierImage;
    CarrierWebsite = carrierWebsite;
    Config = config;
    InputRequiredConfig = inputRequiredConfig;
    CountryId = countryId;
    UpdateOn = DateTime.UtcNow;
    CreatedBy = updatedBy;
  }

  public static Carrier CreateDefaultCarrierForClient(int carrierId, string? name, int? countryId, EmployeeId createBy)
  {
    return new Carrier()
    {
      CarrierId = carrierId,
      Name = name,
      CarrierImage = null,
      CarrierWebsite = null,
      CountryId = countryId,
      IsClientCarrier = true,
      Config = null,
      InputRequiredConfig = null,
      DisplayOrder = null,
      Active = true,
      IsDefault = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createBy
    };
  }
  //for my carriers
  public static int GetNextMyCarrierClientId(Carrier? lastCarrier)
  {
    int carrierId = 10000;
    if (lastCarrier != null)
    {
      carrierId = lastCarrier.CarrierId + 1;
    }
    return carrierId;
  }
  //for default carriers
  public static int GetNextCarrierId(Carrier? lastCarrier)
  {
    int carrierId = 1;
    if (lastCarrier != null)
    {
      carrierId = lastCarrier.CarrierId + 1;
    }
    return carrierId;
  }

  public void UpdateCarrierColors(string? backgroundColor, string? borderColor)
  {
    BackgroundColor = backgroundColor;
    BorderColor = borderColor;
  }
}

