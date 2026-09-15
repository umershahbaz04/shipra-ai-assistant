using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;

namespace Shipra.Backend.API.Application.DTOs.InvoiceUseCase;

public class AdminApiResponseModel<T>
{
  public int totalCount { get; set; }
  public T? list { get; set; }
  public object? result { get; set; }
  public object? message { get; set; }
}

public class InvoiceHistoryDto
{
  public int RowNum { get; set; }
  public int TotalCount { get; set; }
  public string? InvoiceId { get; set; }
  public string? InvoiceNumber { get; set; }
  public string? TenantId { get; set; }
  public string? BillToName { get; set; }
  public string? BillToMobile { get; set; }
  public string? BillToEmail { get; set; }
  public string? BillFromName { get; set; }
  public string? BillFromMobile { get; set; }
  public string? BillFromEmail { get; set; }
  public float Amount { get; set; }
  public string? DueDate { get; set; }
  public float? Discount { get; set; }
  public float? VAT { get; set; }
  public object? SalesTax { get; set; }
  public DateTime CreatedOn { get; set; }
  public string? HostInvoiceURL { get; set; }
  public string? InvoicePDF { get; set; }
  public string? PaymentIntentId { get; set; }
  public int InvoiceStatusId { get; set; }
  public string? StatusValue { get; set; }
}
