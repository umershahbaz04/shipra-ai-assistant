namespace Shipra.Backend.API.Core.DocumentAggregate;

public class DocumentTemplate
{
  public int DocumentTemplateId { get; set; } 
  public int? DocumentTypeId { get; set; } 
  public int? DocumentSizeId { get; set; } 
  public string? Name { get; set; } 
  public string? Description { get; set; } 
  public string? Path { get; set; } 
  public string? SamplePdfUrl { get; set; } 
  public string? SampleImageUrl { get; set; } 
  public bool? Active { get; set; }
}
