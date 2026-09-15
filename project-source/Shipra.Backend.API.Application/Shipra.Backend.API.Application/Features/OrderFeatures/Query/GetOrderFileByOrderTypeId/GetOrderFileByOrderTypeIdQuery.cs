using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetFullfilableOrderFile;
public class GetOrderFileByOrderTypeIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public int OrderTypeId { get; set; } = (int)EnumOrderType.Regular;
}
