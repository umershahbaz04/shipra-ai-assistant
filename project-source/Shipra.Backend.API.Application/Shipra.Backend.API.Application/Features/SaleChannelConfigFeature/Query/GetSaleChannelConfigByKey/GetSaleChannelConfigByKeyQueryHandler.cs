using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigByKey;

public class GetSaleChannelConfigByKeyQueryHandler : RequestHandlerBase<GetSaleChannelConfigByKeyQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public GetSaleChannelConfigByKeyQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelConfigByKeyQueryHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleChannelConfigByKeyQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSCConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigByKey(request.SaleChannelKey!);
      if (oSCConfig is null)
      {
        throw new EntityNotFoundException("SaleChannelConfig ", request.SaleChannelKey!);
      }
      if (string.IsNullOrEmpty(oSCConfig!.Config!))
      {
        throw new EntityNotFoundException("SaleChannelConfig ", oSCConfig!.SaleChannelConfigId!);
      }

      serviceResult = new ServiceResultDTO(oSCConfig!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
