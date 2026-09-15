using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelLookupForSelection;
public class GetAllSaleChannelLookupForSelectionQueryHandler : RequestHandlerBase<GetAllSaleChannelLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public GetAllSaleChannelLookupForSelectionQueryHandler(ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetAllSaleChannelLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSaleChannelLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _SaleChannelConfigRepository.GetAllSaleChannelLookupForSelection();
      data!.Insert(0, new SaleChannelLookup { SaleChannelLookupId = ApplicationConstants.DropDownPlaceHolderId, SaleChannelName = ApplicationConstants.DropDownPlaceHolderName });
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
