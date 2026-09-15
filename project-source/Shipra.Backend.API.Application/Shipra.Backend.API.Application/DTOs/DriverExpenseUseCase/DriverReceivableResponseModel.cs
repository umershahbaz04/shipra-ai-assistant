namespace Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;
public class DriverReceivableResponseModel
{
  public string? DriverReceivableId { get; set; }
  public string? DriverId { get; set; }
  public DateTime? ReceiveDate { get; set; }
  public decimal? Expense { get; set; }
  public decimal? Cash { get; set; }
  public decimal? Total { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? CreatedBy { get; set; }
  public bool? Active { get; set; }
  public string? DriverReceivableNo { get; set; }
  public int? DeliveryNoteId { get; set; }
}
