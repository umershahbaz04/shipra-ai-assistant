using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.DriverAggregate;
public class Driver
{
  public DriverId? DriverId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? DriverCode { get; set; }
  public string AppUsername { get; set; } = null!;
  public string? AppPassword { get; set; }
  public EmployeeId? EmployeeId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }

  public static Driver CreateDriver(ClientId? clientId1, string driverCode, string? appUsername, string? appPassword, EmployeeId? employeeId, EmployeeId? employeeId2)
  {
    return new Driver()
    {
      DriverId = DriverId.New,
      ClientId = clientId1,
      DriverCode = driverCode,
      AppUsername = appUsername!,
      AppPassword = appPassword,
      EmployeeId = employeeId,
      CreatedBy = employeeId2,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void EnableDisableDriver(bool? isActive,EmployeeId updatedBy)
  {
    Active = isActive;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }

  public void UpdateDriver(string? appUsername, string? appPassword, EmployeeId? employeeId, EmployeeId? updatedBy)
  {
    AppUsername = appUsername!;
    AppPassword = appPassword;
    EmployeeId = employeeId!;
    Active = true;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }
}
public sealed record DriverId(Guid Value)
{
  public static DriverId New => new(Guid.NewGuid());
}
