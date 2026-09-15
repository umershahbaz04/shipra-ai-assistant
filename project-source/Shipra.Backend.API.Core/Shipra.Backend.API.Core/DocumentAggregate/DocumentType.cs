namespace Shipra.Backend.API.Core.DocumentAggregate;

public class DocumentType
{
  public int DocumentTypeId { get; set; } 
  public string? Value { get; set; } 
  public bool? HaveSize { get; set; }
}
