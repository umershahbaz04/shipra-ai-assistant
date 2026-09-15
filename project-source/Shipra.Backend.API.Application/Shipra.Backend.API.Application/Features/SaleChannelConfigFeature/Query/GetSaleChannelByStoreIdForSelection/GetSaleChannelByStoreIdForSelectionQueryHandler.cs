using System.Dynamic;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelByStoreIdForSelection;
public class GetSaleChannelByStoreIdForSelectionQueryHandler : RequestHandlerBase<GetSaleChannelByStoreIdForSelectionQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;

  public GetSaleChannelByStoreIdForSelectionQueryHandler(ISaleChannelConfigRepository saleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<GetSaleChannelByStoreIdForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleChannelByStoreIdForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfigList = await _saleChannelConfigRepository.GetSaleChannelByStoreIdForSelection(request.StoreId, _currentUser.ClientId!.Value.ToString(),_currentUser.RoleId);
     
      if (oSaleChannelConfigList is not null && oSaleChannelConfigList.Count > 0)
      {
        dynamic result = new ExpandoObject();
        result.id = ApplicationConstants.DropDownPlaceHolderId;
        result.text = ApplicationConstants.DropDownPlaceHolderName;
        oSaleChannelConfigList!.Insert(0, result);
        serviceResult = new ServiceResultDTO(oSaleChannelConfigList);
        serviceResult.CreateSuccessResponse();
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
