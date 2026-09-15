using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.EnableStoreCommand;
public class EnableStoreCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int StoreId { get; set; }
}
