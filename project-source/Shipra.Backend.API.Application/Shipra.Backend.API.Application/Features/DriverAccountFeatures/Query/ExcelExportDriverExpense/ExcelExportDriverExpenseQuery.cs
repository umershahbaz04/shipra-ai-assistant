using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.ExcelExportDriverExpense;
public class ExcelExportDriverExpenseQuery : IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? DriverId { get; set; }
  public int? ExpenseCategoryId { get; set; }
}
