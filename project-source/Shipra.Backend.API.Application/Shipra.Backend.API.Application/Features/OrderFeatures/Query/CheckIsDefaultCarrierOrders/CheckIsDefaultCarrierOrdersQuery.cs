using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.CheckIsDefaultCarrierOrders;
public class CheckIsDefaultCarrierOrdersQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
public class CheckIsDefaultCarrierOrdersQueryHandler : RequestHandlerBase<CheckIsDefaultCarrierOrdersQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IClientRepository _clientRepository;

  public CheckIsDefaultCarrierOrdersQueryHandler(IOrderRepository orderRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<CheckIsDefaultCarrierOrdersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckIsDefaultCarrierOrdersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
      dynamic res = new ExpandoObject();
      res.IsClientCarrier = false;
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client != null)
      {
        var result = await _orderRepository.GetOrdersByOrderNos(request.OrderNos!, _currentUser.ClientId!);

        var grouped = result.GroupBy(x => x.CarrierId).ToList();
        // Check if any group has more than 1 element based on CarrierId
        bool anyGroupWithMoreThanOneElement = grouped.Count() > 1;
        if (anyGroupWithMoreThanOneElement)
        {
          serviceResultDTO.IsSuccess = false;
          serviceResultDTO.Errors!.Add("MustChooseOneCarrier", new string[] { "Please select one carrier for get awb's" });

        }
        else
        {
          // Get the first CarrierId from the first group with only one element
          var carrierId = grouped!.FirstOrDefault()!.Key; 
          if (carrierId != null)
          {
            if (carrierId == client?.DefaultCarrierId)
            {
              res.IsClientCarrier = true; 
              serviceResultDTO = new ServiceResultDTO(res);
            }
            else
            {
              serviceResultDTO = new ServiceResultDTO(res);

            }
          }
          else
          {
            serviceResultDTO = new ServiceResultDTO(res); 
          }
        }
      }
      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
