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
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReason;
public class GetAllClientReturnReasonQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllClientReturnReasonQueryHandler : RequestHandlerBase<GetAllClientReturnReasonQuery, ServiceResultDTO>
{
  private readonly IReturnRepository _returnRepository;

  public GetAllClientReturnReasonQueryHandler(IReturnRepository returnRepository,IServiceProvider serviceProvider, ILogger<GetAllClientReturnReasonQueryHandler> logger) : base(serviceProvider, logger)
  {
    _returnRepository = returnRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientReturnReasonQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var oExpenseList = await _returnRepository.GetAllClientReturnReason(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientId!.Value.ToString());
       
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
