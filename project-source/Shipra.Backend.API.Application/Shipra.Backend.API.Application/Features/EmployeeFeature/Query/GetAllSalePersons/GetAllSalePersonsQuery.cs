using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllSalePersons;
public class GetAllSalePersonsQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllSalePersonsQueryHandler : RequestHandlerBase<GetAllSalePersonsQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetAllSalePersonsQueryHandler(IEmployeeRepository employeeRepository,IServiceProvider serviceProvider, ILogger<GetAllSalePersonsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllSalePersonsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oEmployes = await _employeeRepository.GetEmployeesForShipperContract(_currentUser.ClientIdStr!); 
      //var oSalePersons = oEmployes!.Where(e => e.SaleChannelConfigId.GetValueOrDefault() > 0).ToList();
      serviceResult = new ServiceResultDTO(oEmployes);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
