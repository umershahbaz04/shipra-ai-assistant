using MediatR;
using Shipra.Backend.API.Application.DTOs;
using System.Collections.Generic;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetMetaFieldsByOrderIds;

public class GetMetaFieldsByOrderIdsQuery : IRequest<ServiceResultDTO>
{
  public List<string>? OrderIds { get; set; }
}
