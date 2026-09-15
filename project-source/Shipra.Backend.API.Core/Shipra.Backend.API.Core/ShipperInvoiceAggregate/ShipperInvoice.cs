using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public partial class ShipperInvoice
{
  public int ShipperInvoiceId { get; private set; }
  public string? InvoiceNo { get; private set; }
  public string? RefNo { get; private set; }
  public Guid? ClientId { get; private set; }
  public int SaleChannelConfigId { get; private set; }
  public decimal? Amount { get; private set; }
  public int? InvoiceStatusId { get; private set; }
  public int? TotalOrder { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public string? CreateBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }

  public static ShipperInvoice Create(string? invoiceNo, Guid? clientId, int saleChannelConfigId, decimal? amount, int? totalOrder, string createdBy)
  {
    if (saleChannelConfigId <= 0)
      throw new ArgumentException("SaleChannelConfigId must be valid.");

    if (amount is not null && amount < 0)
      throw new ArgumentException("Amount cannot be negative.");

    if (totalOrder is not null && totalOrder < 0)
      throw new ArgumentException("TotalOrder cannot be negative.");

    return new ShipperInvoice
    {
      InvoiceNo = Normalize(invoiceNo),
      ClientId = clientId,
      SaleChannelConfigId = saleChannelConfigId,
      Amount = amount ?? 0,
      InvoiceStatusId = (int)EnumInvoiceStatus.Draft,
      TotalOrder = totalOrder ?? 0,
      CreateBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
  }

  public void Update(string? refNo, decimal? amount, int? invoiceStatusId, int? totalOrder, string updatedBy)
  {
    if (amount is not null && amount < 0)
      throw new ArgumentException("Amount cannot be negative.");

    if (totalOrder is not null && totalOrder < 0)
      throw new ArgumentException("TotalOrder cannot be negative.");

    RefNo = Normalize(refNo);
    Amount = amount ?? Amount;
    InvoiceStatusId = invoiceStatusId;
    TotalOrder = totalOrder ?? TotalOrder;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateStatus(int invoiceStatusId, string? refNo, string updatedBy)
  {
    if (invoiceStatusId <= 0)
      throw new ArgumentException("InvoiceStatusId must be valid.");

    InvoiceStatusId = invoiceStatusId;
    RefNo = Normalize(refNo);
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  private static string? Normalize(string? value)
      => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
