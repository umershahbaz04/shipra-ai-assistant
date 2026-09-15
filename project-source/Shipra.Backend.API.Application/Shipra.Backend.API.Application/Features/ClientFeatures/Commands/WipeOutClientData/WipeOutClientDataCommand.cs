using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.WipeOutClientData;

public class WipeOutClientDataCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
    public string? Section { get; set; } = "All";
}
