namespace Shipra.Backend.API.Core.Enum;

public enum EnumSCFolderLookup
{
  Carrier = 1,
  Order = 2,
  CarrierServiceLogo = 3,
  Others = 4,
}
public static class EnumSCFolderLookupHelper
{
  public static string GetEnumString(EnumSCFolderLookup value)
  {
    switch (value)
    {
      case EnumSCFolderLookup.Carrier:
        return "Carrier";
      case EnumSCFolderLookup.Order:
        return "Order";
      case EnumSCFolderLookup.Others:
        return "Others";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumSCFolderLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumSCFolderLookup.Carrier;
      case 2:
        return EnumSCFolderLookup.Order;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
