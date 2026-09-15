using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigById;

public class GetSaleChannelConfigByIdQueryHandler : RequestHandlerBase<GetSaleChannelConfigByIdQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public GetSaleChannelConfigByIdQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelConfigByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleChannelConfigByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSCConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSCConfig is null)
      {
        throw new EntityNotFoundException("SaleChannelConfig ", request.SaleChannelConfigId);
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
