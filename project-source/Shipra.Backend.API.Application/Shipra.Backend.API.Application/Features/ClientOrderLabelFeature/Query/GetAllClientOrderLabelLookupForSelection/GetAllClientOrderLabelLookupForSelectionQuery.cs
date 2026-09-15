using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetAllClientOrderLabelLookupForSelection;
public class GetAllClientOrderLabelLookupForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientOrderLabelLookupForSelectionQueryHandler : RequestHandlerBase<GetAllClientOrderLabelLookupForSelectionQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllClientOrderLabelLookupForSelectionQueryHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetAllClientOrderLabelLookupForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientOrderLabelLookupForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelLookupForSelection(_currentUser.ClientId!);
      var obj = ClientOrderLabelLookup.AddDefault();
      allClientOrderLabels?.Add(obj);
      var list = allClientOrderLabels!.ToList().OrderBy(x => x.ClientOrderLabelLookupId).Select(x => new
      {
        clientOrderLabelLookupId = x.ClientOrderLabelLookupId,
        labelName = x.LabelName,
        colorCode = x.ColorCode
      });
      serviceResult = new ServiceResultDTO(list);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
