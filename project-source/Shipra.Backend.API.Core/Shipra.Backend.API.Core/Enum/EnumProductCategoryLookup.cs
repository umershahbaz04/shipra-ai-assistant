namespace Shipra.Backend.API.Core.Enum;

public enum EnumProductCategoryLookup
{
  General = 1
}
public static class EnumProductCategoryLookupHelper
{
  public static string GetEnumString(EnumProductCategoryLookup value)
  {
    switch (value)
    {
      case EnumProductCategoryLookup.General:
        return "General";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumProductCategoryLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumProductCategoryLookup.General;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
