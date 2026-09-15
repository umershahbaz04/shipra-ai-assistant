using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReason;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllOrderReturn;
public class GetAllOrderReturnQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? ReturnReasonId { get; set; } = null;
  public int? ReturnStatusId { get; set; } = null;
}
public class GetAllOrderReturnQueryHandler : RequestHandlerBase<GetAllOrderReturnQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetAllOrderReturnQueryHandler(IReturnRepository returnRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderReturnQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderReturnQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var oExpenseList = await _returnRepository.GetAllOrderReturn(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString(),request!.ReturnReasonId,request!.ReturnStatusId);

      serviceResult = new ServiceResultDTO(oExpenseList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
