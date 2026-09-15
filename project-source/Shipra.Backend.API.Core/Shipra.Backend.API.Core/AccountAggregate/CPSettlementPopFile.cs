using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.AccountAggregate;
public class CPSettlementPopFile
{
  /// <summary>
  /// CPSettlementPodFileI is a short form form table CarrierPaymentSettlementPodFile
  /// </summary>
  public int CpsettlementPopFileId { get; set; }
  public string? FilePath { get; set; }
  public string? Extension { get; set; }
  public CarrierPaymentSettlementId? CarrierPaymentSettlementId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }

  public static CPSettlementPopFile Create(string url, string extension, CarrierPaymentSettlementId carrierPaymentSettlementId, EmployeeId employeeId)
  {
    return new CPSettlementPopFile
    {
      FilePath = url,
      Extension = extension,
      CarrierPaymentSettlementId = carrierPaymentSettlementId,
      Active = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = employeeId
    };
  }

  public void Delete(EmployeeId? employeeId)
  {
    Active = false;
    UpdatedBy = employeeId;
    UpdateOn = DateTime.UtcNow;
  }
}
