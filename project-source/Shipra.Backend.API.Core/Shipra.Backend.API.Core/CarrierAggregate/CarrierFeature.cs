using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class CarrierFeature : EntityBase, IAggregateRoot
{
  public CarrierFeature() { }
  public CarrierFeatureId? CarrierFeatureId { get; set; } = null!;
  public int? CarrierId { get; set; }
  public string? Feature { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }

  public static CarrierFeature Create(int? carrierId, string? feature, bool active, EmployeeId createdBy)
  {
    return new CarrierFeature()
    {
      CarrierFeatureId = CarrierFeatureId.New,
      Feature = feature,
      CarrierId = carrierId,
      Active = active,
      CreatedBy = createdBy
    };
  }
}
public sealed record CarrierFeatureId(Guid Value)
{
  public static CarrierFeatureId New => new(Guid.NewGuid());
}
