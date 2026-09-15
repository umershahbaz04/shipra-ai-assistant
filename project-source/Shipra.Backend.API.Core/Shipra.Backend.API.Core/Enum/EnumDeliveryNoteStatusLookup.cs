namespace Shipra.Backend.API.Core.Enum;
public enum EnumDeliveryNoteStatusLookup
{
  Pending = 0,
  InProgress = 1,
  Completed = 2
}

public static class EnumDeliveryNoteStatusLookupHelper
{
  public static string GetEnumString(EnumDeliveryNoteStatusLookup value)
  {
    switch (value)
    {
      case EnumDeliveryNoteStatusLookup.InProgress:
        return "InProgress";
      case EnumDeliveryNoteStatusLookup.Completed:
        return "Completed";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumDeliveryNoteStatusLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumDeliveryNoteStatusLookup.InProgress;
      case 2:
        return EnumDeliveryNoteStatusLookup.Completed;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
