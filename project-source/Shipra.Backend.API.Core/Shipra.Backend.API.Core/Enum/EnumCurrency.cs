namespace Shipra.Backend.API.Core.Enum;
public enum EnumCurrency
{
  AED = 1,
  USD = 2,
  GBP = 3
}
public static class EnumCurrencyHelper
{
  public static string GetEnumString(EnumCurrency value)
  {
    switch (value)
    {
      case EnumCurrency.AED:
        return "AED";
      case EnumCurrency.GBP:
        return "GBP";
      case EnumCurrency.USD:
        return "USD";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumCurrency GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumCurrency.AED;
      case 2:
        return EnumCurrency.USD;
      case 3:
        return EnumCurrency.GBP;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
