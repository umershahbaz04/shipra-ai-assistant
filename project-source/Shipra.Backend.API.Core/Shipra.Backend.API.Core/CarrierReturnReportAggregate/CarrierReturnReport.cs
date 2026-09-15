using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.CarrierReturnReportAggregate;
public class CarrierReturnReport
{
  /// <summary>
  /// CarrierReturnReportId
  /// </summary>
  public CarrierRRId? CarrierRrid { get; set; }
  public string? ReturnReportNo { get; set; }
  public int? TotalOrders { get; set; }
  public int? CarrierId { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }

  public static CarrierReturnReport CreateCarrierReturnReport(string returnReportNo, int totalOrders, int? carrierId, EmployeeId? userId)
  {
    return new CarrierReturnReport
    {
      CarrierRrid = CarrierRRId.New,
      CarrierId = carrierId,
      ReturnReportNo = returnReportNo,
      TotalOrders = totalOrders,
      CreatedBy = userId,
      CreatedOn = DateTime.Now
    };
  }

  public static string GetReturnReportNo(int? clientIdentifier)
  {
    string rNo = string.Empty;
    rNo = clientIdentifier + DateTime.Now.ToString("MMddfff");
    return rNo;
  }
}
public sealed record CarrierRRId(Guid Value)
{
  public static CarrierRRId New => new(Guid.NewGuid());
}
