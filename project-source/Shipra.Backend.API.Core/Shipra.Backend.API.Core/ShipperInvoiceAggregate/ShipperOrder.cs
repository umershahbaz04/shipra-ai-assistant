using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.StyledXmlParser.Jsoup.Select;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;
//SELECT so.ShipperOrderId,
//       so.ClientId,
//       so.ShipperInvoiceId,
//       so.InvoiceStatusId,
//       so.SaleChannelConfigId,
//       so.OrderId,
//       so.OrderNo,
//       so.CustomerName,
//       so.CustomerFullAddress,
//       so.Email,
//       so.Mobile1,
//       so.Mobile2,
//       so.StoreName,
//       so.CustomerServiceNo,
//       so.Amount,
//       so.CreatedOn FROM dbo.ShipperOrder AS so
public class ShipperOrder
{
  public long ShipperOrderId { get; private set; }
  public Guid? ClientId { get; private set; }
  public int? ShipperInvoiceId { get; private set; }
  public int? InvoiceStatusId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public Guid? OrderId { get; private set; }
  public string? OrderNo { get; private set; }

  public string? CustomerName { get; set; }
  public string? CustomerFullAddress { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public string? StoreName { get; set; }
  public string? CustomerServiceNo { get; set; }
  public decimal? Amount { get; set; }
  public decimal? Weight { get; set; }

  public DateTime? CreatedOn { get; set; }
  // -----------------------------
  // Factory (Create)
  // -----------------------------
  public static ShipperOrder Create(Order? order, OrderAddress? orderAddress, StoreWitAddresshModel? store)
  {
    if (order is null) throw new ArgumentNullException(nameof(order));
    if (orderAddress is null) throw new ArgumentNullException(nameof(orderAddress));
    if (store is null) throw new ArgumentNullException(nameof(store));

    // If these are required in your domain, validate them explicitly
    if (order.ClientId is null) throw new InvalidOperationException("Order.ClientId is required.");
    if (order.OrderId is null) throw new InvalidOperationException("Order.OrderId is required.");
    if (string.IsNullOrWhiteSpace(order.OrderNo)) throw new InvalidOperationException("Order.OrderNo is required.");

    return new ShipperOrder
    {
      // Core
      ClientId = order.ClientId.Value,
      InvoiceStatusId = (int)EnumInvoiceStatus.Draft,
      ShipperInvoiceId = null,

      SaleChannelConfigId = order.SaleChannelConfigId,
      OrderId = order.OrderId.Value,
      OrderNo = order.OrderNo,

      // Customer snapshot (from address)
      CustomerName = orderAddress.CustomerName,
      CustomerFullAddress = orderAddress.CustomerFullAddress,
      Email = NullIfEmpty(orderAddress.Email),
      Mobile1 = NullIfEmpty(orderAddress.Mobile1),
      Mobile2 = NullIfEmpty(orderAddress.Mobile2),
      Weight = order.Weight,

      // Store snapshot (from store model)
      StoreName = NullIfEmpty(store.StoreName!),
      CustomerServiceNo = NullIfEmpty(store.CustomerServiceNo),

      // Amount (choose correct source field name from your Order)
      Amount = order.Amount,
      CreatedOn = DateTime.UtcNow
    };
  }


  // -----------------------------
  // Update helpers (optional)
  // -----------------------------
  public void UpdateInvoice(int? shipperInvoiceId, int? invoiceStatusId)
  {
    ShipperInvoiceId = shipperInvoiceId;
    InvoiceStatusId = invoiceStatusId;
  }
  public void UpdateSaleChannel(Order? order, OrderAddress? orderAddress, StoreWitAddresshModel? store)
  {
    if (order is null) throw new ArgumentNullException(nameof(order));
    if (orderAddress is null) throw new ArgumentNullException(nameof(orderAddress));
    if (store is null) throw new ArgumentNullException(nameof(store));

    SaleChannelConfigId = order.SaleChannelConfigId;

    CustomerName = orderAddress.CustomerName;
    CustomerFullAddress = orderAddress.CustomerFullAddress;
    Email = NullIfEmpty(orderAddress.Email);
    Mobile1 = NullIfEmpty(orderAddress.Mobile1);
    Mobile2 = NullIfEmpty(orderAddress.Mobile2);
    Weight = order.Weight;

    // Store snapshot (from store model)
    StoreName = NullIfEmpty(store.StoreName!);
    CustomerServiceNo = NullIfEmpty(store.CustomerServiceNo);

    // Amount (choose correct source field name from your Order)
    Amount = order.Amount;
  }

  private static string? NullIfEmpty(string? value)
    => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

}
