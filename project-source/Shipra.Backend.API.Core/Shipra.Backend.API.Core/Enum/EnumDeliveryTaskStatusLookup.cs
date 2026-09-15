namespace Shipra.Backend.API.Core.Enum;

public enum EnumDeliveryTaskStatusLookup
{
  Unallocated = 1,
  Allocated = 2,
  Completed = 3,
  Attempted = 4
}
public static class EnumDeliveryTaskStatusHelper
{
  public static string GetEnumString(EnumDeliveryTaskStatusLookup value)
  {
    switch (value)
    {
      case EnumDeliveryTaskStatusLookup.Unallocated:
        return "Pending";
      case EnumDeliveryTaskStatusLookup.Allocated:
        return "Started";
      case EnumDeliveryTaskStatusLookup.Completed:
        return "Completed";
      case EnumDeliveryTaskStatusLookup.Attempted:
        return "Attempted";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumDeliveryTaskStatusLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumDeliveryTaskStatusLookup.Unallocated;
      case 2:
        return EnumDeliveryTaskStatusLookup.Allocated;
      case 3:
        return EnumDeliveryTaskStatusLookup.Completed;
      case 4:
        return EnumDeliveryTaskStatusLookup.Attempted;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
