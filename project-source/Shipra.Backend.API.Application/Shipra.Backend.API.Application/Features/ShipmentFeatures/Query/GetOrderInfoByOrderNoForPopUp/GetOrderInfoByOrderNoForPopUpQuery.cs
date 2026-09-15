using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetOrderInfoByOrderNoForPopUp;
public class GetOrderInfoByOrderNoForPopUpQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}

