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
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CheckDuplicationCarrierAliasByClient;
public class CheckDuplicationCarrierAliasByClientCommand : IRequest<ServiceResultDTO>
{
  public int? CarrierId { get; set; }
  public string? CarrierAlias { get; set; }
  public int CarrierContractTypeId { get; set; } = (int)EnumCarrierContractType.OwnContractType; 
}
public class CheckDuplicationCarrierAliasByClientCommandHandler : RequestHandlerBase<CheckDuplicationCarrierAliasByClientCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public CheckDuplicationCarrierAliasByClientCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<CheckDuplicationCarrierAliasByClientCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckDuplicationCarrierAliasByClientCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {
        ShipraContractCarrier oActiveCarrierAlias = await _carrierRepository.GetShipraContractCarrierByAlias(request.CarrierAlias,request.CarrierId);
        if (oActiveCarrierAlias is not null)
        {
          serviceResult.IsSuccess = false;
          serviceResult.CreateError("AlreadyExist", new string[] { "Carrier with id " + request.CarrierId! + " Already Exist." });
        }
      }
      else
      {
        ActiveCarrier oActiveCarrierAlias = await _carrierRepository.GetActiveCarrierByAlias(request.CarrierAlias, _currentUser.ClientId);
        if (oActiveCarrierAlias is not null)
        {
          serviceResult.IsSuccess = false;
          serviceResult.CreateError("AlreadyExist", new string[] { "Nick Name with " + request.CarrierAlias! + " Already Exist." });
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
public class CheckDuplicationCarrierAliasByClientCommandValidator : AbstractValidator<CheckDuplicationCarrierAliasByClientCommand>
{
  public CheckDuplicationCarrierAliasByClientCommandValidator()
  {
    RuleFor(x => x.CarrierAlias).NotEmpty().NotNull();
  }
}
