using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.AccountAggregate;
public class CarrierPaymentSettlement
{
  public CarrierPaymentSettlementId? CarrierPaymentSettlementId { get; set; }
  public decimal? Amount { get; set; }
  public decimal? AmountReceived { get; set; }
  public int? PaymentStatusId { get; set; }
  public string? PaymentRef { get; set; }
  public int CarrierId { get; set; }
  public DateTime? PaymentDate { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }

  public static CarrierPaymentSettlement CreateCarrierPaymentSettlement(decimal? amount, string? paymentRef, DateTime? paymentDate, int carrierId, EmployeeId employeeId)
  {
    return new CarrierPaymentSettlement()
    {
      CarrierPaymentSettlementId = CarrierPaymentSettlementId.New,
      Amount = amount,
      PaymentRef = paymentRef,
      PaymentDate = paymentDate,
      PaymentStatusId = (int)EnumPaymentStatus.Unpaid,
      CarrierId = carrierId,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = employeeId
    };
  }
  public void UpdateCarrierPaymentSettlement(decimal? amount, string? paymentRef, DateTime? paymentDate, EmployeeId employeeId)
  {
    Amount = amount;
    PaymentRef = paymentRef;
    PaymentDate = paymentDate;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
  public void UpdateAmountReceived(decimal? amount, EmployeeId employeeId)
  {
    AmountReceived = amount;
    PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }

  public void MarkCarrierSettlementPaid(EmployeeId? employeeId)
  {
    PaymentStatusId = (int)EnumPaymentStatus.Paid;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }  
  public void MarkCarrierSettlementUnPaid(EmployeeId? employeeId)
  {
    PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
}
public sealed record CarrierPaymentSettlementId(Guid Value)
{
  public static CarrierPaymentSettlementId New => new(Guid.NewGuid());
}
