namespace Shipra.Backend.API.Core.Models;

public class CreateOrderFollowUpRequestModel
{ 
  public string? OrderNo { get; set; } 
  public string? ClientId { get; set; } 
  public string? Status { get; set; } 
  public DateTime? LastUpdate { get; set; }
}
