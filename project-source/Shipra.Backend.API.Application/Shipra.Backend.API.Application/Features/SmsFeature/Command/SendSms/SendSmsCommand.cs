using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.SmsFeature.Command.SendSms;
public class SendSmsCommand : IRequest<ServiceResultDTO>
{
}
public class SendSmsCommandHandler : RequestHandlerBase<SendSmsCommand, ServiceResultDTO>
{
  private readonly ISmsService _smsService;
  private readonly ISMSProcessRepository _smsProcessRepository;

  public SendSmsCommandHandler(ISmsService smsService, ISMSProcessRepository sMSProcessRepository, IServiceProvider serviceProvider, ILogger<SendSmsCommandHandler> logger) : base(serviceProvider, logger)
  {
    _smsService = smsService;
    _smsProcessRepository = sMSProcessRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SendSmsCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
     await _smsService.SendSms(new Message(), _currentUser.ClientId!); 

      await Task.Delay(2);
    }
    catch (Exception)
    {

      throw;
    }
    return serviceResultDTO;
  }
}
