using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderTypeLookupUseCase;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.DTOs.PaymentMethodLookupUseCase;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using DocumentFormat.OpenXml.Bibliography;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Application.Features.OrderTypeLookupFeatures.Query.GetAllOrderTypeLookupQuery;
public class GetAllOrderTypeLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>>>
{
}

public class GetAllOrderTypeLookupQueryHandler : RequestHandlerBase<GetAllOrderTypeLookupQuery, ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>>>
{
  private readonly IOrderBoxRepository _orderBoxRepository;
  private readonly IOrderTypeLookupRepository _orderTypeLookupRepository;

  public GetAllOrderTypeLookupQueryHandler(IOrderBoxRepository orderBoxRepository, IOrderTypeLookupRepository orderTypeLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderTypeLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
    _orderTypeLookupRepository = orderTypeLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>>> HandleRequest(GetAllOrderTypeLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>> serviceResult = new ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>>();

    try
    {
      #region check default box and create 
      var allClientOrderBox = await _orderBoxRepository.GetAllClientClientOrderBox(_currentUser.ClientId);
      if (allClientOrderBox.Count == 0)
      {
        List<BoxTypeLookup> boxTypeLookups = await _orderBoxRepository.GetAllBoxTypeLookup();
        foreach (var boxtype in boxTypeLookups)
        {
          ClientOrderBox clientOrderBox = ClientOrderBox.Create(_currentUser.ClientId, boxtype.BoxName, boxtype.Length, boxtype.Width, boxtype.Height, boxtype.Volume, boxtype.IsDefault, _currentUser.EmployeeId!);
          var existOClientTax = await _orderBoxRepository.CreateClientOrderBox(clientOrderBox);
        }

      }
      #endregion
      var data = await _orderTypeLookupRepository.GetAllOrderTypeLookup();
      var obj = OrderTypeLookup.AddDefault();
      data?.Add(obj);
      var responseDto = data!.Select(x => new OrderTypeLookupResponseModel() { OrderTypeId = x.OrderTypeId, OrderTypeName = x.OrderTypeName }).OrderBy(x => x.OrderTypeId).ToList();

      serviceResult = new ServiceResultDTOWithTypeModel<List<OrderTypeLookupResponseModel>>(responseDto);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

