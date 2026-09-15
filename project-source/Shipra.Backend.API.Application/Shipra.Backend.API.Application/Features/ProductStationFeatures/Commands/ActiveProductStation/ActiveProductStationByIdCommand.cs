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

namespace Shipra.Backend.API.Application.Features.ProductStationFeatures.Commands.ActiveProductStation;
public class ActiveProductStationByIdCommand : IRequest<ServiceResultDTO>
{
  public int ProductStationId { get; set; }
}
public class ActiveProductStationByIdCommandHandler : RequestHandlerBase<ActiveProductStationByIdCommand,ServiceResultDTO>
{
  private readonly IProductStationRepository _productStationRepository;

  public ActiveProductStationByIdCommandHandler(IProductStationRepository productStationRepository,IServiceProvider serviceProvider, ILogger<ActiveProductStationByIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productStationRepository = productStationRepository;
  }
   

  protected override async Task<ServiceResultDTO> HandleRequest(ActiveProductStationByIdCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var target = await _productStationRepository.GetProductStationById(request.ProductStationId);
      if (target == null)
      {
        throw new EntityNotFoundException("Product Station", request.ProductStationId);
      }
      target.MarkAsActive(_currentUser.EmployeeId);
      response.IsSuccess = await _productStationRepository.UpdateProductStation(target);

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }

  }
}
