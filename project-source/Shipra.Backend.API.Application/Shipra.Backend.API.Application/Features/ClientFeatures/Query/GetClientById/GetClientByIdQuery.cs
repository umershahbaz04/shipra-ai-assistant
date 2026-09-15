using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientById;
public class GetClientByIdQuery : IRequest<ServiceResultDTOWithTypeModel<ClientResponseModel>>
{
  public  string? ClientId { get; set; }
}
