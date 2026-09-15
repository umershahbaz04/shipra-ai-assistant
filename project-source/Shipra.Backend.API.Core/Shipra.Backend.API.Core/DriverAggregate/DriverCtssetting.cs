using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.DriverAggregate;
public partial class DriverCTSSetting
{
  public DriverCTSSettingId? DriverCTSSettingId { get; set; } 
  public ClientId? ClientId { get; set; } 
  public int? CarrierTrackingStatusId { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; } 
  public bool? Active { get; set; }

  public static DriverCTSSetting Create(ClientId? clientId, int carrierTrackingStatusId,EmployeeId? createdBy)
  {
    return new DriverCTSSetting()
    {
      DriverCTSSettingId = DriverCTSSettingId.New,
      ClientId = clientId,
      CarrierTrackingStatusId = carrierTrackingStatusId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
    };
  }
}
public sealed record DriverCTSSettingId(Guid Value)
{
  public static DriverCTSSettingId New => new(Guid.NewGuid());
}
