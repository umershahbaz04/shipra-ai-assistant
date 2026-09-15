namespace Shipra.Backend.API.Application.DTOs.Common.Request;

public class FilterModelDTO
{
  public DateTime? CreatedFrom { get; set; } = null;
  public DateTime? CreatedTo { get; set; }=null;
  public int Start { get; set; } =0;
  public int Length { get; set; } = 10;
  public string? Search { get; set; } = string.Empty;
  public string? SortDir { get; set; } = "Desc";
  public int SortCol { get; set; } = 0;
}
