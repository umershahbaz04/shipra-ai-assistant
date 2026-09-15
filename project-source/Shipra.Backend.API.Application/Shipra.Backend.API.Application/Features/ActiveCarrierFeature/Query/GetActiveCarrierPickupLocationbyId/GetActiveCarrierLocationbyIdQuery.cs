using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationbyId;
public class GetActiveCarrierLocationbyIdQuery : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierPickupLocationId { get; set; }
}

