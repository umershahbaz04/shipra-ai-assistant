namespace Shipra.Backend.API.Core.Enum;
public enum EnumDeliveryNoteDetailStatusLookup
{
  Pending = 1,
  Completed = 2,
  Attempted = 3
}

public static class EnumDeliveryNoteDetailStatusLookupHelper
{
  public static string GetEnumString(EnumDeliveryNoteDetailStatusLookup value)
  {
    switch (value)
    {
      case EnumDeliveryNoteDetailStatusLookup.Pending:
        return "Pending";
      case EnumDeliveryNoteDetailStatusLookup.Completed:
        return "Completed";
      case EnumDeliveryNoteDetailStatusLookup.Attempted:
        return "Attempted";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumDeliveryNoteDetailStatusLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumDeliveryNoteDetailStatusLookup.Pending;
      case 2:
        return EnumDeliveryNoteDetailStatusLookup.Completed;
      case 3:
        return EnumDeliveryNoteDetailStatusLookup.Attempted;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
