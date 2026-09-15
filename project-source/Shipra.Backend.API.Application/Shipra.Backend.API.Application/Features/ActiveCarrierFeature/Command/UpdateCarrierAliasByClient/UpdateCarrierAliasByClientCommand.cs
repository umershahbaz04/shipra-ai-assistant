using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateCarrierAliasByClient;
public class UpdateCarrierAliasByClientCommand : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierId { get; set; }
  public string? CarrierAlias { get; set; }
}
public class UpdateCarrierAliasByClientCommandHandler : RequestHandlerBase<UpdateCarrierAliasByClientCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdateCarrierAliasByClientCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateCarrierAliasByClientCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateCarrierAliasByClientCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      ActiveCarrier? oActiveCarrier = await _carrierRepository.GetActiveCarrierById(request.ActiveCarrierId, _currentUser.ClientId);

      if (oActiveCarrier is null)
      {
        throw new EntityNotFoundException("ActiveCarrier", request.ActiveCarrierId!);
      }
      else
      {
        ActiveCarrier oActiveCarrierAlias = await _carrierRepository.GetActiveCarrierByCarrierAlias(oActiveCarrier.CarrierId,request.CarrierAlias, _currentUser.ClientId);

        if (oActiveCarrierAlias is not null)
        {
          serviceResult.CreateError("AlreadyExist", new string[] { "Nick with name " + request.CarrierAlias! + " Already Exist." });
        }
        else
        {
          oActiveCarrier!.UpdateCarrierAlias(request.CarrierAlias, _currentUser.EmployeeId);
          await _carrierRepository.UpdateActiveCarrier(oActiveCarrier!);
        }
      } 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateCarrierAliasByClientCommandValidator : AbstractValidator<UpdateCarrierAliasByClientCommand>
{
  public UpdateCarrierAliasByClientCommandValidator()
  {
    RuleFor(x => x.CarrierAlias).NotEmpty().NotNull();  
  }
}
