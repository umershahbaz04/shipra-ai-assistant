using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Services.Implementation.Modified;
public interface ISaleChannelService
{
  Task CreateSaleChannelOrderById(List<CreateOrderResponseDetailModel>? createdOrders, CreateSaleChannelRequestModel createSaleChannel);
  //Task ProcessOrdersAsync(int? saleChannelConfigId, ClientId clientId, Store oStore, SaleChannelConfig oSaleChannelConfig, SaleChannelConfigResponseModel oSaleChannelsResponse, string orderType, EmployeeId employeeId, bool isAutoFulfill);
  Task ProcessSaleChannelOrdersAsync(IEnumerable<SaleChannelConfigResponseModel> filterdOrderData);
}
