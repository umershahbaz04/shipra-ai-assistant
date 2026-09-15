using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;
public class CreateOrderClientSideRequestModel
{
  public  List<OrderAddressClientSideAbcModel>? orderList { get; set; }
  public List<SettingConfigItem>? settingConfig { get; set; }
  public bool? IsSaleChannelOrder { get; set; } = false; 
  public long? OrderDraftId { get; set; } = 0; 
  public bool? IsAssigCarrier { get; set; } = false; 
  public bool? WithThirdPartyResponse { get; set; } = false; 
}

public class OrderAddressClientSideAbcModel : CreateUpdateOrderCommonRequestModel
{
  public OrderAddressClientSideModel? OrderAddress { get; set; } = new();
}
public class OrderAddressClientSideModel : OrderAddressModel
{
  public new string? CityId { get; set; }
  public new string? AreaId { get; set; }  // This is a string in client-side request  
  public new string? ProvinceId { get; set; }
  public new string? PinCodeId { get; set; }
  public new string? StateId { get; set; } 
}
