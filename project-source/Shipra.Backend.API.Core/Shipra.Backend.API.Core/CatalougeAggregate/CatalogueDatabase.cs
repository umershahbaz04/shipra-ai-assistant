namespace Shipra.Backend.API.Core.CatalougeAggregate;
public class CatalogueDatabase
{
  public int DatabaseId { get; set; } 
  public string? ConnectionString { get; set; }  
  public string? IpAddress { get; set; } 
  public int? MaxClientAllowed { get; set; } 
  public int? QuantityUsed { get; set; }
  public bool? Active { get; set; }
  public int? OperationalStatusId { get; set; }

  public void UpdateUsedQty(int? newQty)
  {
    QuantityUsed = newQty;
  }
}
