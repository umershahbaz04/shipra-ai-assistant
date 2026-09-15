namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public class ShipperRate
{
  public long ShipperRateId { get; private set; }
  public Guid? ClientId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public int? ServiceRateGroupId { get; private set; }
  public decimal? AdditionalRate { get; private set; }
  public long? ContractShipperRatesId { get; private set; } 
  public string? EmployeeName { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public Guid? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public Guid? UpdatedBy { get; private set; }
  public bool? Active { get; private set; }


  public static ShipperRate Create(Guid? clientId,int? saleChannelConfigId,int? serviceRateGroupId, decimal? additionalRate,string? employeeName, long? contractShipperRatesId,Guid createdBy)
  {
    return new ShipperRate
    {
      ClientId = clientId,
      SaleChannelConfigId = saleChannelConfigId,
      ServiceRateGroupId = serviceRateGroupId,
      AdditionalRate = additionalRate,
      ContractShipperRatesId = contractShipperRatesId,
      EmployeeName = employeeName,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }
   
  public void Update(decimal? additionalRate,long? contractShipperRatesId,bool? active,Guid updatedBy)
  {
    AdditionalRate = additionalRate;
    ContractShipperRatesId = contractShipperRatesId;
    Active = active;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
   
  public void Deactivate(Guid updatedBy)
  {
    Active = false;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
}  
