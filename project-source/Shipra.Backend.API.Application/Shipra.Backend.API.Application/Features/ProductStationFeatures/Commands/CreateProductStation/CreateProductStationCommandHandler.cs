using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.CreateProductStation;
public class CreateProductStationCommandHandler : RequestHandlerBase<CreateProductStationCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IProductStationRepository _productStationRepository;

  public CreateProductStationCommandHandler(IClientRepository clientRepository, IProductStationRepository productStationRepository, IServiceProvider serviceProvider, ILogger<CreateProductStationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _productStationRepository = productStationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateProductStationCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    { 
      var isExistStation = await _productStationRepository.IsProductStationExist(request.Name!, _currentUser.ClientId!);

      if (!isExistStation)
      {
        var pstationCode = await _productStationRepository.GetNextProductStationCode(_currentUser.ClientId!);
        var productStation = ProductStation.CreateProductStation(pstationCode, request.Name, _currentUser.ClientId!, _currentUser.EmployeeId!, request.IsDefault);
        var createdProductStation = await _productStationRepository.CreateProductStation(productStation);
        
        //Check if at the time of create Product station user's select mark as defaul or not?
        if (request!.IsDefault.GetValueOrDefault(false))
        {
          //Remove existing Product Station from default
          var defaultProductStation = await _productStationRepository.GetDefaultProductStation(_currentUser.ClientId!);
          defaultProductStation!.RemoveDefaultProductStation(_currentUser.EmployeeId);
          await _productStationRepository.UpdateProductStation(defaultProductStation);

          //Set newly create Product Station as default
          createdProductStation?.MarkAsDefaultProductStation(_currentUser.EmployeeId);
          await _productStationRepository.UpdateProductStation(createdProductStation!);

          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client != null)
          {
            //Update client to set default Product Station
            client?.UpdateClientDefaultProductStation(createdProductStation?.ProductStationId, _currentUser.EmployeeId);
            await _clientRepository.UpdateClient(client!);
          }
        }
        response.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        response = new ServiceResultDTO(new BaseResponseDto()
        {
          Data = false,
          Message = "Product station already exist"
        });
      }
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }

  }
}
