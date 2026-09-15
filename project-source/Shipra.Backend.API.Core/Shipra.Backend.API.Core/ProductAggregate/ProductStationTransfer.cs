using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductStationTransfer : EntityBase, IAggregateRoot
{
  public ProductStationTransfer() { }

  public ProductStaionTransferId? ProductStaionTransferId { get; private set; }
  public string? TransferNo { get; private set; } 
  public string? TrackingNo { get; private set; } 
  public int? OriginProductStationId { get; private set; } 
  public int? DestinationProductStationId { get; private set; } 
  public DateTime? ExpectedArrivalTime { get; private set; } 
  public int? TransferStatusId { get; private set; } 
  public DateTime? CreatedOn { get; private set; } 
  public EmployeeId? CreatedBy { get; private set; } 
  public EmployeeId? UpdatedBy { get; private set; } 
  public DateTime? UpdatedOn { get; private set; }

  public static ProductStationTransfer CreateStationTransfer(string? trackingNo, int? originProductStationId, int? destinationProductStationId, DateTime? expectedArrivalTime, int? transferStatusId, EmployeeId? createdBy)
  {
    return new ProductStationTransfer()
    {
      ProductStaionTransferId = ProductStaionTransferId.New,
      //TransferNo = generating Random no,
      TrackingNo = trackingNo,
      OriginProductStationId = originProductStationId,
      DestinationProductStationId = destinationProductStationId,
      ExpectedArrivalTime = expectedArrivalTime,
      TransferStatusId = transferStatusId,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,

    };
  }
}
public sealed record ProductStaionTransferId(Guid Value)
{
  public static ProductStaionTransferId New => new(Guid.NewGuid());
}
