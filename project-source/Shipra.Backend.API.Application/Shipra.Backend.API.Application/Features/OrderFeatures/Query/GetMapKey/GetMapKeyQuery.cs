using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapKey;
public class GetMapKeyQuery : IRequest<ServiceResultDTO>
{
}
public class GetMapKeyQueryHandler : RequestHandlerBase<GetMapKeyQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetMapKeyQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetMapKeyQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetMapKeyQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var key = await _countryRepository.GetMapKeys();

      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = key! });

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
    }
    return serviceResult;
  }
}
