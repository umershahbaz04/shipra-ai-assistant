using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelsByStoreId;
public class GetAllSaleChannelsByStoreIdQuery : IRequest<ServiceResultDTO>
{
  public string? StoreId { get; set; }
}
public class GetAllSaleChannelsByStoreIdQueryHandler : RequestHandlerBase<GetAllSaleChannelsByStoreIdQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;

  public GetAllSaleChannelsByStoreIdQueryHandler(ISaleChannelConfigRepository saleChannelConfigRepository, IServiceProvider serviceProvider, ILogger <GetAllSaleChannelsByStoreIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSaleChannelsByStoreIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfigList = await _saleChannelConfigRepository.GetAllSaleChannelForSelection(request.StoreId,_currentUser.ClientId!.Value.ToString());
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
