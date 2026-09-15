using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder; 
using OrderItemModel = Shipra.Backend.API.Application.DTOs.OrderUseCase.OrderItemModel;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
public class UpdateOrderCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
  public int? StoreId { get; set; }
  //public int? SaleChannelConfigId { get; set; }
  public int? OrderTypeId { get; set; }
  public DateTime? OrderDate { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; } 
  public decimal? Amount { get; set; } 
  public decimal? CShippingCharges { get; set; }
  public int? PaymentStatusId { get; set; } 
  public decimal? Weight { get; set; }
  public decimal? ItemValue { get; set; }
  public int? OrderRequestVia { get; set; }
  public int? PaymentMethodId { get; set; }
  public int StationId { get; set; }
  public decimal Discount { get; set; }
  public decimal VAT { get; set; }
  public string? RefNo { get; set; }
  public long? OrderDraftId { get; set; } = 0;
  public int? SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public List<OrderBoxRequestModel>? OrderBoxs { get; set; } = new(); 
  public OrderNoteModel? OrderNote { get; set; } 
  public OrderAddressModel? OrderAddress { get; set; }
  public List<SettingConfigItem>? settingConfig { get; set; }
  public List<OrderItemModel>? OrderItems { get; set; } = new();
  public List<OrderTaxModel>? OrderTaxes { get; set; } = new(); 
}

