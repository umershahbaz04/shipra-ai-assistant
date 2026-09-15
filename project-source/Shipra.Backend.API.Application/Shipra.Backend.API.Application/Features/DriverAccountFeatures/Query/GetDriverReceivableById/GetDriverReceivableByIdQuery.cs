using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverReceivableById;
public class GetDriverReceivableByIdQuery : IRequest<ServiceResultDTOWithTypeModel<DriverReceivableResponseModel>>
{
  public string? DriverReceivableId { get; set; }
}
