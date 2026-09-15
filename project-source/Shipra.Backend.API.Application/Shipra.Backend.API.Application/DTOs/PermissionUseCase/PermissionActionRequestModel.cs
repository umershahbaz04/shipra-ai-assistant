namespace Shipra.Backend.API.Application.DTOs.PermissionUseCase;
public class PermissionActionRequestModel
{
  public string? Controller { get; set; }
  public List<string>? Actions { get; set; } = new List<string>();
}
