using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllCODCollectionPendingsMyCarrier;
public class GetAllCODCollectionPendingsMyCarrierQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? StoreId { get; set; }
  public int? OrderTypeId { get; set; }
}
public class GetAllCODCollectionPendingsMyCarrierQueryHandler : RequestHandlerBase<GetAllCODCollectionPendingsMyCarrierQuery, ServiceResultDTO>
{
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public GetAllCODCollectionPendingsMyCarrierQueryHandler(IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<GetAllCODCollectionPendingsMyCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCODCollectionPendingsMyCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      dynamic data = await _deliveryTaskRepository.GetAllCODCollectionPendingsMyCarrier(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.OrderTypeId);

      serviceResult = new ServiceResultDTO(data);
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
