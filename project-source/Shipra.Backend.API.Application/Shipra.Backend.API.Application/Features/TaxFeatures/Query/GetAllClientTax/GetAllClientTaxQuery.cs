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
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetAllClientTax;
public class GetAllClientTaxQuery : CommonFilterModel,IRequest<ServiceResultDTO>
{

}
public class GetAllClientTaxQueryHandler : RequestHandlerBase<GetAllClientTaxQuery, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public GetAllClientTaxQueryHandler(ITaxRepository taxRepository,IServiceProvider serviceProvider, ILogger<GetAllClientTaxQueryHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientTaxQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _taxRepository.GetAllClientTaxes(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
