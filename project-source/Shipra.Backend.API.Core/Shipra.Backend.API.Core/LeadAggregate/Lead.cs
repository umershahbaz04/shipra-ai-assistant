using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.LeadAggregate;

public class Lead
{
  public Lead() { }
  public LeadId? LeadId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? PhoneNumber { get; set; }
  public string? ProductName { get; set; } 
  public string? GoogleLocationLink { get; set; } 
  public EmployeeId? SalespersonId { get; set; }
  public int? LeadStatusId { get; set; }
  public int? CountryId { get; set; }
  public bool? Active { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public long? OrderDraftId { get; set; }
  public Guid? OrderId { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }

  public static Lead CreateLead(ClientId clientId, string phoneNumber, string productName, string googleLocationLink, EmployeeId? salespersonId, EmployeeId? createdBy, int clientLeadStatusId, int? countryId)
  {
    return new Lead()
    {
      LeadId = LeadAggregate.LeadId.New,
      ClientId = clientId,
      PhoneNumber = phoneNumber,
      ProductName = productName, 
      GoogleLocationLink = googleLocationLink, 
      SalespersonId = salespersonId,
      LeadStatusId = clientLeadStatusId,
      CountryId = countryId,
      Active = true,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }

  public void UpdateLeadStatus(int? leadStatusId, EmployeeId? updatedBy)
  {
    LeadStatusId = leadStatusId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void AssignSalesperson(EmployeeId? salespersonId, EmployeeId? updatedBy)
  {
    SalespersonId = salespersonId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
}

public sealed record LeadId(Guid Value)
{
  public static LeadId New => new(Guid.NewGuid());
}
