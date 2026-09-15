using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;
public partial class ShipperInvoiceAdjustment
{
  public int ShipperInvoiceAdjustmentId { get; private set; }
  public int? ShipperInvoiceId { get; private set; }
  public int? TransactionTypeId { get; private set; }
  public decimal? Amount { get; private set; }
  public string? Comment { get; private set; }
  public Guid? ClientId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public string? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }

 
  public static ShipperInvoiceAdjustment Create(int? transactionTypeId,decimal? amount, string? comment,Guid? clientId,int? saleChannelConfigId,string? createdBy)
  {
    if (amount == 0)
      throw new ArgumentException("Adjustment amount cannot be zero.");

    if (saleChannelConfigId <= 0)
      throw new ArgumentException("SaleChannelConfigId must be valid.");

    return new ShipperInvoiceAdjustment
    { 
      TransactionTypeId = transactionTypeId,
      Amount = amount,
      Comment = Normalize(comment),
      ClientId = clientId,
      SaleChannelConfigId = saleChannelConfigId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }
   
  public void Update(decimal amount,string? comment,int? transactionTypeId,string updatedBy)
  {
    if (amount == 0)
      throw new ArgumentException("Adjustment amount cannot be zero.");

    Amount = amount;
    Comment = Normalize(comment);
    TransactionTypeId = transactionTypeId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateShipperInvoiceId(int shipperInvoiceId,string? updatedBy)
  {
    ShipperInvoiceId = shipperInvoiceId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
  private static string? Normalize(string? value)
      => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
