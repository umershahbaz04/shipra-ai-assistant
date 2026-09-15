using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelLookupById;
public class GetSaleChannelLookupByIdQueryHandler : RequestHandlerBase<GetSaleChannelLookupByIdQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public GetSaleChannelLookupByIdQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelLookupByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleChannelLookupByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var sclookup = await _SaleChannelConfigRepository.GetSaleChannelLookupById(request.SaleChannelLookupId);

      if (sclookup is null)
      {
        throw new EntityNotFoundException("SaleChannelLookup ", request.SaleChannelLookupId);
      }
      if (string.IsNullOrEmpty(sclookup!.InputRequiredConfig!))
      {
        throw new EntityNotFoundException("SaleChannelLookup ", sclookup!.InputRequiredConfig!);
      }

      serviceResult = new ServiceResultDTO(sclookup!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
