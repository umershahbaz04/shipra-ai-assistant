using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.DeleteStoreCommand;
public class DeleteStoreCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int StoreId { get; set; }
}

