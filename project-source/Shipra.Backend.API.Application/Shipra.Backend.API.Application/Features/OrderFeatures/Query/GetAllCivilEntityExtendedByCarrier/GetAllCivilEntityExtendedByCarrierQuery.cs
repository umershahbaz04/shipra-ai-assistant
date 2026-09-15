using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllCivilEntityExtendedByCarrier;
public class GetAllCivilEntityExtendedByCarrierQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetAllCivilEntityExtendedByCarrierQueryHandler : RequestHandlerBase<GetAllCivilEntityExtendedByCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllCivilEntityExtendedByCarrierQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetAllCivilEntityExtendedByCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCivilEntityExtendedByCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      List<CivilEntityExtended> civilEntityExtendeds = await _carrierRepository.GetCivilEntityExtended(request.CarrierId);
      serviceResult = new ServiceResultDTO(civilEntityExtendeds);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAllCivilEntityExtendedByCarrierQueryValidator : AbstractValidator<GetAllCivilEntityExtendedByCarrierQuery>
{
  public GetAllCivilEntityExtendedByCarrierQueryValidator()
  {
    RuleFor(x => x.CarrierId).NotNull().NotEmpty().GreaterThan(0);
  }
}
