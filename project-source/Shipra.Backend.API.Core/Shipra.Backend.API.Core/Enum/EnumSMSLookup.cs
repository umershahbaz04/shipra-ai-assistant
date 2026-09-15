namespace Shipra.Backend.API.Core.Enum;

public enum EnumSMSLookup
{
  CountrySms = 1,
  SmartSms = 2,
}
public static class EnumSMSLookupHelper
{
  public static string GetEnumString(EnumSMSLookup value)
  {
    switch (value)
    {
      case EnumSMSLookup.CountrySms:
        return "Country Sms";
      case EnumSMSLookup.SmartSms:
        return "Smart Sms";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumSMSLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumSMSLookup.CountrySms;
      case 2:
        return EnumSMSLookup.SmartSms;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
