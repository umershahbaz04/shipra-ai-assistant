using System.Dynamic;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelByStoreIdForSelection;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelByLookupIdForSelection;
public class GetAllSaleChannelByLookupIdForSelectionQueryHandler : RequestHandlerBase<GetAllSaleChannelByLookupIdForSelectionQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;

  public GetAllSaleChannelByLookupIdForSelectionQueryHandler(ISaleChannelConfigRepository saleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelByStoreIdForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSaleChannelByLookupIdForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfigList = await _saleChannelConfigRepository.GetAllSaleChannelByLookupIdForSelection(request.SaleChannelLookupId, _currentUser.ClientId!.Value.ToString());

      dynamic result = new ExpandoObject();
      result.id = ApplicationConstants.DropDownPlaceHolderId;
      result.text = "All Sale Channel(s)";
      oSaleChannelConfigList!.Insert(0, result);
      serviceResult = new ServiceResultDTO(oSaleChannelConfigList);
      serviceResult.CreateSuccessResponse();

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
