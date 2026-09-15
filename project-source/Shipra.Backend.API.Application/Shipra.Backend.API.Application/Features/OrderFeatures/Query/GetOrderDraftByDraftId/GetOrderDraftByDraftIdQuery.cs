using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderDraftByDraftId;
public class GetOrderDraftByDraftIdQuery : IRequest<ServiceResultDTO>
{
  public long OrderDraftId { get; set; }
}
public class GetOrderDraftByDraftIdQueryHandler : RequestHandlerBase<GetOrderDraftByDraftIdQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetOrderDraftByDraftIdQueryHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetOrderDraftByDraftIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderDraftByDraftIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orderDraft = await _orderRepository.GetDraftOrderById(request.OrderDraftId,_currentUser.ClientId!);
      var orderInfoJson = orderDraft?.OrderInfo;
      if (string.IsNullOrWhiteSpace(orderInfoJson))
        return serviceResult; // return if empty
      var jObject = JObject.Parse(orderInfoJson!);
      var orderAddressJson = jObject["OrderAddress"]?.ToString();
      var orderAddress = JsonConvert.DeserializeObject<OrderAddressModel>(orderAddressJson!);
      var oAddressMap = ConvertToResponseModel(orderAddress!);
      if (oAddressMap != null)
      {
        jObject["OrderAddress"] = JObject.FromObject(oAddressMap);
        orderDraft!.OrderInfo = jObject.ToString(Formatting.None);
      }
       
      if (orderDraft is null)
      {
        throw new EntityNotFoundException("OrderDraft ", request.OrderDraftId!);
      }
      serviceResult = new ServiceResultDTO(new
      {
        orderDraft.OrderDraftId,
        orderDraft.OrderNo,
        orderDraft.OrderInfo,
        orderDraft.OrderTypeId,
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  #region ConvertResponse Model
  public static OrderAddressWithEntityResponseModel? ConvertToResponseModel(OrderAddressModel orderAddress)
  {
    if (orderAddress == null) return null;

    var entityMappings = CivilEntityHelper.GetEntityMappings(orderAddress.EntityAddressDataJson);
    // Ensure default values when JSON is null or does not contain a mapping
    if (string.IsNullOrEmpty(orderAddress.EntityAddressDataJson))
    {
      CivilEntityHelper.EnsureDefaultEntityMappings(entityMappings, orderAddress);
    }
    return new OrderAddressWithEntityResponseModel
    {
      OrderAddressId = orderAddress.OrderAddressId,
      CustomerName = orderAddress.CustomerName,
      //CustomerFullAddress = orderAddress.CustomerFullAddress,
      Email = orderAddress.Email,
      Mobile1 = orderAddress.Mobile1,
      Mobile2 = orderAddress.Mobile2,
      Country = orderAddress.CountryId,
      City = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.City),
      Area = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Area),
      Province = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Province),
      State = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.State),
      PinCode = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.PinCode),
      StreetAddress = orderAddress.StreetAddress,
      StreetAddress2 = orderAddress.StreetAddress2,
      HouseNo = orderAddress.HouseNo,
      BuildingName = orderAddress.BuildingName,
      Landmark = orderAddress.Landmark,
      Latitude = orderAddress.Latitude,
      Longitude = orderAddress.Longitude,
      EntityAddressDataJson = orderAddress.EntityAddressDataJson,
      SelectedCarrierId = orderAddress.SelectedCarrierId
    };
  }
  #endregion
}

#region Address Model
public class OrderAddressModel
{
  public int OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public int? SelectedCarrierId { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? EntityAddressDataJson { get; set; }

  public List<EntityAddressModel>? EntityAddressList { get; set; }
}
public class EntityAddressModel
{
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public int? EntityId { get; set; }
  public int? EntityTypeId { get; set; }
}
#endregion
