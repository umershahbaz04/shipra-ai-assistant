using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ActivityLogAggregate;
public class ActivityLog
{
  public ActivityLogId? ActivityLogId { get; private set; }
  public string? Request { get; private set; }
  public string? Response { get; private set; }
  public string? Aggregate { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? EmployeeId { get; private set; }
  public DateTime? CreateOn { get; private set; }

  public static ActivityLog Create(string? request, string? response, string? aggregate, ClientId? clientId, EmployeeId? employeeId, DateTime? createOn)
  {
    return new ActivityLog()
    {
      ActivityLogId = ActivityLogId.New,
      Request = request,
      Response = response,
      Aggregate = aggregate,
      ClientId = clientId,
      EmployeeId = employeeId,
      CreateOn = createOn
    };
  }

}
public sealed record ActivityLogId(Guid Value)
{
  public static ActivityLogId New => new(Guid.NewGuid());
}
