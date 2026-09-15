using MediatR;
using Shipra.Backend.API.Application.DTOs;
using System.Collections.Generic;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.AssignStationToOrders;

public class AssignStationToOrdersCommand : IRequest<ServiceResultDTO>
{
    public List<string> OrderIds { get; set; } = new List<string>();
    public int StationId { get; set; }
}
