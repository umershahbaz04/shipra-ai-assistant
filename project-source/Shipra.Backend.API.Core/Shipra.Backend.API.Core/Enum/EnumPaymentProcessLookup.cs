namespace Shipra.Backend.API.Core.Enum;
public enum EnumPaymentProcessLookup
{
  Stripe = 1
}
public static class EnumPaymentProcessLookupHelper
{
  public static string GetEnumString(EnumPaymentProcessLookup value)
  {
    switch (value)
    {
      case EnumPaymentProcessLookup.Stripe:
        return "Stripe";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumPaymentProcessLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumPaymentProcessLookup.Stripe;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
