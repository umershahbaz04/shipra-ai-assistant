using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelConfig;

public class GetAllSaleChannelConfigQueryHandler : RequestHandlerBase<GetAllSaleChannelConfigQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public GetAllSaleChannelConfigQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetAllSaleChannelConfigQueryHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSaleChannelConfigQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _SaleChannelConfigRepository.GetAllSaleChannelConfig(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
