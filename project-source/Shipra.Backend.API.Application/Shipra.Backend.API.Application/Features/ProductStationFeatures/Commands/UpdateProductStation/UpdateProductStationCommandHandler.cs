using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.UpdateProductStation;
public class UpdateProductStationCommandHandler : RequestHandlerBase<UpdateProductStationCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IProductStationRepository _productStationRepository;

  public UpdateProductStationCommandHandler(IClientRepository clientRepository, IProductStationRepository productStationRepository, IServiceProvider serviceProvider, ILogger<UpdateProductStationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _productStationRepository = productStationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateProductStationCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var productStation = await _productStationRepository.GetProductStationById(request.ProductStationId);
      if (productStation == null)
      {
        throw new EntityNotFoundException("Product Station", request.ProductStationId);
      } 

      productStation?.UpdateProductStation(request.ProductStationId, request.Name, _currentUser.EmployeeId!);
      await _productStationRepository.UpdateProductStation(productStation!);
      //Check if at the time of create Product station user's select mark as defaul or not?
      if (request!.IsDefault.GetValueOrDefault(false))
      {
        //Remove existing Product Station from default
        var defaultProductStation = await _productStationRepository.GetDefaultProductStation(_currentUser.ClientId!);
        if (defaultProductStation is not null)
        {
          defaultProductStation!.RemoveDefaultProductStation(_currentUser.EmployeeId);
          await _productStationRepository.UpdateProductStation(defaultProductStation);

          //Set newly create Product Station as default
          productStation?.MarkAsDefaultProductStation(_currentUser.EmployeeId);
          await _productStationRepository.UpdateProductStation(productStation!);

          var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
          if (client != null)
          {
            //Update client to set default Product Station
            client?.UpdateClientDefaultProductStation(productStation?.ProductStationId, _currentUser.EmployeeId);
            await _clientRepository.UpdateClient(client!);
          }
        }
        else
        {
          //if no station already default
          productStation?.MarkAsDefaultProductStation(_currentUser.EmployeeId);
          await _productStationRepository.UpdateProductStation(productStation!);
        }
      }
      else
      { 
        productStation?.UpdateProductStation(request.ProductStationId, request.Name, _currentUser.EmployeeId!, request.IsDefault);
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
