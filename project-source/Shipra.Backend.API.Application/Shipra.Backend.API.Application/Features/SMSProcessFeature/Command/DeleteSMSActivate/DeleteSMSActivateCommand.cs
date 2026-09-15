using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.DeleteSMSActivate;

public class DeleteSMSActivateCommand : IRequest<ServiceResultDTO>
{
  public int SMSActivateId { get; set; }
}
