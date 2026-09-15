using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetAllClientOrderLabelLookup;
public class GetAllClientOrderLabelLookupQuery : CommonFilterModel, IRequest<ServiceResultDTO>
{
}
public class GetAllClientOrderLabelLookupQueryHandler : RequestHandlerBase<GetAllClientOrderLabelLookupQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllClientOrderLabelLookupQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllClientOrderLabelLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientOrderLabelLookupQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      dynamic data = await _orderRepository.GetAllClientOrderLabelLookup(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString());

      serviceResultDTO = new ServiceResultDTO(data);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
