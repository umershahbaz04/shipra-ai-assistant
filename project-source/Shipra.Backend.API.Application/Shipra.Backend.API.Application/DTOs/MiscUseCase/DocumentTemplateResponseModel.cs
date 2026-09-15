using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Application.DTOs.MiscUseCase;

public class DocumentTemplateResponseModel : DocumentTemplate
{
  public bool IsSelected { get; set; }
}
