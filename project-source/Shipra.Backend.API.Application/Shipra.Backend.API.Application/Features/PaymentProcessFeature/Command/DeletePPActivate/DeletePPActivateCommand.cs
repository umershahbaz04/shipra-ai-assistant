using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.DeletePPActivate;
public class DeletePPActivateCommand : IRequest<ServiceResultDTO>
{
  public int PpactivateId { get; set; }
}
