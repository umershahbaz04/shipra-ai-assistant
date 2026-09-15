using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderBoxFeature.Query.GetAllClientOrderBox;
public class GetAllClientOrderBoxQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientOrderBoxQueryHandler : RequestHandlerBase<GetAllClientOrderBoxQuery, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;

  public GetAllClientOrderBoxQueryHandler(IOrderBoxRepository orderBoxRepository,IServiceProvider serviceProvider, ILogger<GetAllClientOrderBoxQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientOrderBoxQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      
      var data = await _orderBoxRepository.GetAllClientClientOrderBox(_currentUser.ClientId!); 

      var orderMap = _mapper.Map<List<ClientOrderBoxResponseModel>>(data);  
      serviceResult = new ServiceResultDTO(orderMap);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
