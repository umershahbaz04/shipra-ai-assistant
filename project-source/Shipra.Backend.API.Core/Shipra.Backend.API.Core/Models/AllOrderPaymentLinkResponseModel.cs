using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class AllOrderPaymentLinkResponseModel
{
  public int RowNum { get; set; } // Corresponds to ROW_NUMBER() OVER
  public int TotalCount { get; set; } // Corresponds to COUNT(*) OVER
  public string? PaymentLinkId { get; set; } // pl.PaymentLinkId
  public string? ServiceUUId { get; set; } // pl.ServiceUUId
  public string? PaymentLinkUrl { get; set; } // pl.PaymentLinkUrl
  public string? TrackingUrl { get; set; } // pl.PaymentLinkUrl
  public decimal? Amount { get; set; } // pl.PaymentLinkUrl
  public string? OrderNo { get; set; } // o.OrderNo
  public string? CustomerName { get; set; } // oa.CustomerName
  public string? Email { get; set; } // oa.Email
  public string? StatusName { get; set; } // plsl.StatusName
  public bool? AllowRefreshPaymentStatus { get; set; } // plsl.StatusName
  public DateTime CreatedOn { get; set; } // pl.CreatedOn
  public DateTime? PaymentReleaseDate { get; set; } // pl.PaymentReleaseDate (nullable)
  public DateTime? ScheduledPayoutDate { get; set; } // pl.PaymentReleaseDate (nullable)
  public DateTime? PaidOn { get; set; } // pl.PaidOn (nullable)
}
public class OrderResponseModel
{ // never remove any property from this model, as it is used in multiple places
  public int RowNum { get; set; }
  public int TotalCount { get; set; }
  public string? OrderId { get; set; }
  public string? OrderNo { get; set; }
  public string? RefNo { get; set; }
  public int? NumberOfPieces { get; set; }
  public DateTime? OrderDate { get; set; }
  public DateTime? CreatedOn { get; set; }
  public decimal? Amount { get; set; }
  public string? DeliveryTypeName { get; set; }
  public string? CarrierTrackingNo { get; set; }
  public string? CarrierTrackingStatus { get; set; }
  public int? CarrierTrackingStatusId { get; set; }
  public string? TrackingStatus { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public decimal? Weight { get; set; }
  public string? SaleChannelName { get; set; }
  public string? StoreName { get; set; }
  public string? StoreImage { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? CustomerName { get; set; }
  public string? CustomerFullAddress { get; set; }
  public string? FullAddress { get; set; }
  public string? Mobile1 { get; set; }
  public decimal? DeliveryCharges { get; set; }
  public decimal? CShippingCharges { get; set; }
  public bool? IsClientCarrier { get; set; }   
  public int? ItemsCount { get; set; }
  public string? PaymentStatus { get; set; } 
  public decimal? ItemValue { get; set; }  
  public string? PaymentMethodId { get; set; } 
  public string? Carrier { get; set; }
  public string? OrderTypeName { get; set; }
  public string? ClientId { get; set; }
  public int? DatabaseId { get; set; }
  public string? InvoiceStatus { get; set; } = "UnInvoiced";
  public string? CarrierPayment { get; set; } = "Unpaid"; //default value 
  public decimal? ThirdpartyCharges { get; set; } 
  public string? InvoiceNo { get; set; }
  public decimal? CarrierPaymentRate { get; set; }
  public string? FromName { get; set; }
  public string? ToName { get; set; }
  public decimal? CustomerLatitude { get; set; }
  public decimal? CustomerLongitude { get; set; }
  public int? SaleChannelConfigId { get; set; }
}

