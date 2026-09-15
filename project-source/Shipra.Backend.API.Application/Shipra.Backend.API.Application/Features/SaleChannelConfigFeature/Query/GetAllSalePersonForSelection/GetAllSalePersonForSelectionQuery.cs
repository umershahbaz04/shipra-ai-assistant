using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSalePersonForSelection;
public class GetAllSalePersonForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllSalePersonForSelectionQueryHandler : RequestHandlerBase<GetAllSalePersonForSelectionQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetAllSalePersonForSelectionQueryHandler(IEmployeeRepository employeeRepository,IServiceProvider serviceProvider, ILogger<GetAllSalePersonForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSalePersonForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oEmployes = await _employeeRepository.GetAllSalesPersonForSelection(_currentUser.ClientId);
      serviceResult = new ServiceResultDTO(oEmployes.Select(x =>  new
      {
        id = x.EmployeeId?.Value.ToString(),
        text = x.EmployeeCode + " | "  + x.EmployeeName 
      }));
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
