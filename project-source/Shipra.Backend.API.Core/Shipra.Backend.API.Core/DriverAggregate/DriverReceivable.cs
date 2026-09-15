using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.DriverAggregate;
public class DriverReceivable
{
  public DriverReceivableId? DriverReceivableId { get; private set; }

  public DriverId? DriverId { get; private set; }
  public DeliveryNoteId? DeliveryNoteId { get; private set; }
  public DateTime? ReceiveDate { get; private set; }
  public decimal? Expense { get; private set; }
  public decimal? Cash { get; private set; }
  public decimal? Total { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public bool? Active { get; private set; }
  public string? DriverReceivableNo { get; private set; }
  public int? DriverPaidStatusId { get; private set; }

  public static DriverReceivable CreateDriverRecivable(DriverId driverId, decimal? expense, decimal? cash, decimal? total, EmployeeId createdBy, DeliveryNoteId deliveryNoteId)
  {
    return new DriverReceivable()
    {
      DriverReceivableId = DriverReceivableId.New,
      DriverReceivableNo = GetDriverReceivableNo(),
      ReceiveDate = DateTime.UtcNow,
      DeliveryNoteId = deliveryNoteId,
      DriverId = driverId,
      Expense = expense,
      Cash = cash,
      Total = total,
      Active = true,
      DriverPaidStatusId = (int)EnumDriverPaidStatus.UnPaid,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
  }

  public static string? GetDriverReceivableNo()
  {
    return $"DRS{DateTime.Now.ToString("yyMMddssff")}";
  }

  public void UpdateDriverRecivable(DriverId driverId, DateTime? receiveDate, decimal? expense, decimal? cash, decimal? total, EmployeeId updatedBy, bool? active, DeliveryNoteId? deliveryNoteId)
  {
    DriverId = driverId;
    ReceiveDate = receiveDate;
    Expense = expense;
    Cash = cash;
    Total = total;
    Active = active;
    DeliveryNoteId = deliveryNoteId;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }

  public void UpdateDriverRecivablePaidStatus(int? driverPaidStatusId, EmployeeId updatedBy)
  {
    DriverPaidStatusId = driverPaidStatusId;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }
}
public sealed record DriverReceivableId(Guid Value)
{
  public static DriverReceivableId New => new(Guid.NewGuid());
}
