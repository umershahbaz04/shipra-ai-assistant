using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Query.GetPDFDriverExpenseReport;
public class GetPDFDriverExpenseReportQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? DriverId { get; set; }
  public int? ExpenseCategoryId { get; set; }
}
