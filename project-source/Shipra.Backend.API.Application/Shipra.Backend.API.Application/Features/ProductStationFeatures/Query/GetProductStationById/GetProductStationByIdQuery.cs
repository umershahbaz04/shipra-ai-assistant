using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Example;
using Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Query.GetProductStationById;
public class GetProductStationByIdQuery : IRequest<ServiceResultDTOWithTypeModel<ProductStationResposeModel>>
{
  public int ProductStationId { get; set; }
}
