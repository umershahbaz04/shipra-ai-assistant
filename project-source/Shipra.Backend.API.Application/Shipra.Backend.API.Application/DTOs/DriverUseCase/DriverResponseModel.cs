namespace Shipra.Backend.API.Application.DTOs.DriverUseCase;
public class DriverResponseModel
{
  public string? DriverId { get; set; }
  public string? ClientId { get; set; }
  public string? DriverCode { get; set; }
  public string AppUsername { get; set; } = null!;
  public string? AppPassword { get; set; }
  public string? EmployeeId { get; set; }
  public string? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }
}
