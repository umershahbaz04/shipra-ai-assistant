using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.LeadAggregate;

public class ClientLeadStatusLookup
{
    public int ClientLeadStatusId { get; set; }
    public ClientId? ClientId { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
    public EmployeeId? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public EmployeeId? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public static ClientLeadStatusLookup Create(string description, int? displayOrder, ClientId clientId, EmployeeId createdBy)
    {
        return new ClientLeadStatusLookup
        {
            Description = description,
            DisplayOrder = displayOrder,
            ClientId = clientId,
            CreatedBy = createdBy,
            CreatedOn = DateTime.UtcNow,
            Active = true
        };
    }

    public void Update(string description, int? displayOrder, EmployeeId updatedBy)
    {
        Description = description;
        DisplayOrder = displayOrder;
        UpdatedBy = updatedBy;
        UpdatedOn = DateTime.UtcNow;
    }

    public void ActivateDeactive(EmployeeId updatedBy, bool isActive)
    {
        Active = isActive;
        UpdatedBy = updatedBy;
        UpdatedOn = DateTime.UtcNow;
    }
}
