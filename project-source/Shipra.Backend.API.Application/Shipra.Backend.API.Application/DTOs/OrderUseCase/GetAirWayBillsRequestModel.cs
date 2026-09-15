using System.ComponentModel.DataAnnotations;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class GetAirWayBillsRequestModel
{
  [Required]
  public string? OrderNos { get; set; }
  [RequiredDecimalGreaterThanZero]
  public int AwbTypeId { get; set; }
}
public sealed class RequiredDecimalGreaterThanZero : ValidationAttribute
{
  /// <summary>
  /// Designed for dropdowns to ensure that a selection is valid and not the dummy "SELECT" entry
  /// </summary>
  /// <param name="value">The integer value of the selection</param>
  /// <returns>True if value is greater than zero</returns>
  public override bool IsValid(object? value)
  {
    // return true if value is a non-null number > 0, otherwise return false
    decimal i;
    return value != null && decimal.TryParse(value.ToString(), out i) && i > 0;
  }
}
