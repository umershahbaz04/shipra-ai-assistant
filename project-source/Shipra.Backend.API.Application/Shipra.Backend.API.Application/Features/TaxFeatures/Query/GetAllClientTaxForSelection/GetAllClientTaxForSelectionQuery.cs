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
using Shipra.Backend.API.Core.TaxAggregate;

namespace Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetAllClientTaxForSelection;
public class GetAllClientTaxForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientTaxForSelectionQueryHandler : RequestHandlerBase<GetAllClientTaxForSelectionQuery, ServiceResultDTO>
{
  private readonly ITaxRepository _taxRepository;

  public GetAllClientTaxForSelectionQueryHandler(ITaxRepository taxRepository,IServiceProvider serviceProvider, ILogger<GetAllClientTaxForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _taxRepository = taxRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientTaxForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _taxRepository.GetAllTaxTypeLookup(); 
      list.Add(TaxTypeLookup.AddDefault());
      var newList = list.OrderBy(x => x.TaxId).ToList();
      serviceResult = new ServiceResultDTO(newList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
