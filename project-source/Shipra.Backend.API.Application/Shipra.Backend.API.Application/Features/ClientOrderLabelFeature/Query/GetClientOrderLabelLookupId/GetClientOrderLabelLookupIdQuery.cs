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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetClientOrderLabelLookupId;
public class GetClientOrderLabelLookupIdQuery :IRequest<ServiceResultDTO>
{
  public int ClientOrderLabelLookupId { get; set; }
}
public class GetClientOrderLabelLookupIdQueryHandler : RequestHandlerBase<GetClientOrderLabelLookupIdQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetClientOrderLabelLookupIdQueryHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetClientOrderLabelLookupIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientOrderLabelLookupIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelLookupForSelection(_currentUser.ClientId!);

      var oClientOrderLabelLookup = allClientOrderLabels.FirstOrDefault(x => x.ClientOrderLabelLookupId == request.ClientOrderLabelLookupId);
      if (oClientOrderLabelLookup is null)
      {
        throw new EntityNotFoundException("ClientOrderLabelLookup ", request.ClientOrderLabelLookupId!);
      }
      serviceResult = new ServiceResultDTO(oClientOrderLabelLookup); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
