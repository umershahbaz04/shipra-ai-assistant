using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.DocumentAggregate;

public class DocumentTemplateConfig
{
  public DocumentTemplateConfigId? DocumentTemplateConfigId { get; set; }
  public ClientId? ClientId { get; set; }
  public int? DocumentTemplateId { get; set; }
  public string? TemplateName { get; set; }
  public bool? IsAskEveryTime { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public bool? Active { get; set; }

  public static DocumentTemplateConfig Create(ClientId? ClientId, int documentTemplateId, string? templateName,EmployeeId employeeId)
  {
    return new DocumentTemplateConfig
    {
      DocumentTemplateConfigId = DocumentTemplateConfigId.New,
      ClientId = ClientId,
      DocumentTemplateId = documentTemplateId,
      TemplateName = templateName,
      IsAskEveryTime = false,
      CreatedBy = employeeId,
      CreatedOn = DateTime.UtcNow,
    };
  }

  public void Update(string? templateName, int documentTemplateId,bool? isAskEveryTime, EmployeeId? employeeId)
  {
    TemplateName = templateName;
    DocumentTemplateId = documentTemplateId;
    IsAskEveryTime = isAskEveryTime;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
}
public sealed record DocumentTemplateConfigId(Guid Value)
{
  public static DocumentTemplateConfigId New => new(Guid.NewGuid());
}
