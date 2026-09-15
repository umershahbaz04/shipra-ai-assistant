using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public partial class ReturnTrackingHistory
{
  public ReturnTrackingHistoryId? ReturnTrackingHistoryId { get; set; } 
  public ReturnId? ReturnId { get; set; } 
  public int? ReturnStatusLookupId { get; set; }  
  public string? CreatedByName { get; set; } 
  public string? Comment { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; }

  public static  ReturnTrackingHistory Create(ReturnId returnId, int? returnStatusLookupId, string createdByName,string? comment = null)
  {
    return new ReturnTrackingHistory()
    {
      ReturnTrackingHistoryId = ReturnTrackingHistoryId.New,
      ReturnId = returnId,
      ReturnStatusLookupId = returnStatusLookupId,
      CreatedByName = createdByName,
      Comment = comment,
      CreatedOn = DateTime.UtcNow
    };
  }
}
public sealed record ReturnTrackingHistoryId(Guid Value)
{
  public static ReturnTrackingHistoryId New => new(Guid.NewGuid());
}
