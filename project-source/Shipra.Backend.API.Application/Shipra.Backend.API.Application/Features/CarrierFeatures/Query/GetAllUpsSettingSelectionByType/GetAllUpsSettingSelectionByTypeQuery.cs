using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.Services.Implementation.Factory;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllUpsSettingSelectionByType;
public class GetAllUpsSettingSelectionByTypeQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
  public string? Type { get; set; }
}
public class GetAllUpsSettingSelectionByTypeQueryHandler : RequestHandlerBase<GetAllUpsSettingSelectionByTypeQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly CarrierServiceFactory _carrierHandlerFactory;


  public GetAllUpsSettingSelectionByTypeQueryHandler(ICarrierRepository carrierRepository, CarrierServiceFactory carrierHandlerFactory, IServiceProvider serviceProvider, ILogger<GetAllUpsSettingSelectionByTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _carrierHandlerFactory = carrierHandlerFactory;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllUpsSettingSelectionByTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      await Task.Delay(1);
      var handler = _carrierHandlerFactory.Create(request.CarrierId);  
      var data =  handler!.GetDataByType(request.Type!);

      serviceResult = new ServiceResultDTO(data!); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

