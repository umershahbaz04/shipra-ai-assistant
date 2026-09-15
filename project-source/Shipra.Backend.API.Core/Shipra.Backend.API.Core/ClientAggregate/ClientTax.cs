using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientTax
{
  public int ClientTaxId { get; set; } 
  public ClientId? ClientId { get; set; } 
  public int? TaxId { get; set; } 
  public decimal? Percentage { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; } 
  public DateTime? UpdateOn { get; set; } 
  public bool? Active { get; set; }

  public static ClientTax CreateClientText(int? taxId, decimal? percentage,ClientId clientId, EmployeeId? employeeId)
  {
    return new ClientTax
    {
      TaxId = taxId,
      Percentage = percentage,
      ClientId = clientId,
      CreatedBy = employeeId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void EnableDisableTax(bool? isActive, EmployeeId? employeeId)
  {
    Active = isActive;
    UpdatedBy = employeeId;
    UpdateOn = DateTime.UtcNow;
  }

  public void UpdateClientText(int? taxId, decimal? percentage, EmployeeId? employeeId)
  {
    TaxId = taxId;
    Percentage = percentage;
    UpdatedBy = employeeId;
    UpdateOn = DateTime.UtcNow;
  }
}
