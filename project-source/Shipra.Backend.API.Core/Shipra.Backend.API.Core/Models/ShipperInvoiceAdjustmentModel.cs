using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ShipperInvoiceAdjustmentModel
{
  public int? RowNum { get; set; }
  public int? TotalCount { get; set; }
  public int? ShipperInvoiceAdjustmentId { get; set; }
  public int? ShipperInvoiceId { get; set; }
  public int? TransactionTypeId { get; set; }
  public decimal? Amount { get; set; }
  public string? Comment { get; set; }
  public string? StatusName { get; set; }
  public string? ClientId { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public DateTime CreatedOn { get; set; }
  public string? CreatedBy { get; set; }
  public string? InvoiceNo { get; set; }

  // Transaction Type Details
  public string? Name { get; set; }
  public string? Description { get; set; }
}
public class ShipperInvoiceListRow
{
  public long RowNum { get; set; }           // ROW_NUMBER() returns bigint
  public int TotalCount { get; set; }        // COUNT(*) OVER() returns int 
  public string? InvoiceNo { get; set; }
  public string? RefNo { get; set; } 
  public int? TotalOrder { get; set; }       // if column can be null
  public decimal? Amount { get; set; }       // money/decimal 
  public string? StatusName { get; set; } 
  public int ShipperInvoiceId { get; set; }  // adjust to long if your PK is bigint
  public string? EmployeeName { get; set; } 
  public DateTime? CreatedOn { get; set; }   // use DateTime if NOT nullable in DB
}
