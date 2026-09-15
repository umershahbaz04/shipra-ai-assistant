using MediatR;
using System.Collections.Generic;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetAllDriverReceivable;
public class GetAllDriverReceivableQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public List<Guid>? DriverIds { get; set; }
}


