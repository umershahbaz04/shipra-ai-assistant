using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierLocationByCarrierAndCountry;
public class GetCarrierLocationByCarrierQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetCarrierLocationByCarrierQueryHandler : RequestHandlerBase<GetCarrierLocationByCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetCarrierLocationByCarrierQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetCarrierLocationByCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierLocationByCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oCarrier = await _carrierRepository.GetCarrierById(request.CarrierId);
      if (oCarrier is not null)
      {
        var carrierLocation = await _carrierRepository.GetCarrierLocationByCarrier(oCarrier!);
        if (carrierLocation is null)
        {
          throw new EntityNotFoundException("CarrierLocation ", request.CarrierId); 
        }
        serviceResult = new ServiceResultDTO(carrierLocation);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.ErrorEntityNotFound));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetCarrierLocationByCarrierQueryValidator : AbstractValidator<GetCarrierLocationByCarrierQuery>
{
  public GetCarrierLocationByCarrierQueryValidator()
  {
    RuleFor(x => x.CarrierId).NotEmpty().NotNull().GreaterThan(0);
  }
}
