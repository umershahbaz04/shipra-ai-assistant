using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

public class GetAllDriverExpenseQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? DriverId { get; set; }
  public int? ExpenseCategoryId { get; set; }
}
