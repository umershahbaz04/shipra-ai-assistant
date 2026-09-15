namespace Shipra.Backend.API.Application.DTOs.Common.Request;
public class FilterDateClientModel
{
  public DateTime? CreatedFrom { get; set; } = null;
  public DateTime? CreatedTo { get; set; } = null; 
}
