using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class ReturnActivityLog
{
  public ReturnActivtyLogId? ReturnActivtyLogId { get; set; } 
  public string? ActivityLog { get; set; } 
  public ReturnId? ReturnId { get; set; } 
  public DateTime CreatedOn { get; set; }

  public static ReturnActivityLog Create(ReturnId? returnId,string? activityLog)
  {
    return new ReturnActivityLog
    {
      ReturnActivtyLogId = ReturnActivtyLogId.New,
      ActivityLog = activityLog,
      ReturnId = returnId,
      CreatedOn = DateTime.UtcNow,
    };
  }
}
public sealed record ReturnActivtyLogId(Guid Value)
{
  public static ReturnActivtyLogId New => new(Guid.NewGuid());
}
