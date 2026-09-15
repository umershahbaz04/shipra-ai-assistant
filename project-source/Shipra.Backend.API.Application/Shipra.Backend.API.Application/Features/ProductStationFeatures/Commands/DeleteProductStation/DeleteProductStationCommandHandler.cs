using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.DeleteProductStation;
public class DeleteProductStationCommandHandler : RequestHandlerBase<DeleteProductStationCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IProductStationRepository _productStationRepository;

  public DeleteProductStationCommandHandler(IProductStationRepository productStationRepository,IServiceProvider serviceProvider, ILogger<DeleteProductStationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productStationRepository = productStationRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteProductStationCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      var target = await _productStationRepository.GetProductStationById(request.ProductStationId);
      if (target == null)
      {
        throw new EntityNotFoundException("Product Station", request.ProductStationId);
      }
      if (target.IsDefault.GetValueOrDefault())
      {
        response?.CreateError("DefaultStation", new string[] { "You cannot delete default station." });
        return response!;
      }
      target.MarkAsInActive(_currentUser.EmployeeId);
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
