using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Application.DTOs.MiscUseCase;

/// <summary>
/// ////////
/// </summary>


public class DocumentSizeResponseModel : DocumentSize
{
  public bool IsSelected { get; set; }
}
