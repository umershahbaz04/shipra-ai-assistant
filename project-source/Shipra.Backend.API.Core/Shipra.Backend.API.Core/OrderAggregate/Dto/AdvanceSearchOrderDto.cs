using System;

namespace Shipra.Backend.API.Core.OrderAggregate.Dto;

public class AdvanceSearchOrderDto
{
  public long RowNum { get; set; }
  public int TotalCount { get; set; }
  public Guid OrderId { get; set; }
  public string? OrderNo { get; set; }
  public string? CustomerName { get; set; }
  public string? Mobile1 { get; set; }
  public string? RefNo { get; set; }
  public string? CarrierTrackingNo { get; set; }
  public DateTime? OrderDate { get; set; }
  public decimal? Amount { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
}
