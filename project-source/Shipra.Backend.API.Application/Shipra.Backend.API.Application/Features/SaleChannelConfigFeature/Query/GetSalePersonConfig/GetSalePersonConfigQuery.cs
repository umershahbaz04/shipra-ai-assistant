using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSalePersonConfig;
public class GetSalePersonConfigQuery : IRequest<ServiceResultDTO>
{
}
public class GetSalePersonConfigQueryHandler : RequestHandlerBase<GetSalePersonConfigQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public GetSalePersonConfigQueryHandler(IClientRepository clientRepository,IStoreRepository storeRepository, ISaleChannelConfigRepository saleChannelConfigRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetSalePersonConfigQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _storeRepository = storeRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSalePersonConfigQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Employee oEmployee = await _employeeRepository.GetEmployeeById(_currentUser.EmployeeId!, _currentUser.ClientId!);
      if (oEmployee == null)
      {
        serviceResult.CreateError("EmployeeNotFound", new string[] { "Employee not found" });
        return serviceResult;
        //employee not found
      }

      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientId!);
      }

      SaleChannelConfig? oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(oEmployee!.SaleChannelConfigId.GetValueOrDefault(), _currentUser.ClientId!);

      if (oSaleChannelConfig == null)
      {
        serviceResult.CreateError("SaleChannelConfig", new string[] { "SaleChannel Configuration not found" });
        return serviceResult;
        //sale channel is not found
      }

      Store? oStore = await _storeRepository.GetStoreById(oSaleChannelConfig!.StoreId.GetValueOrDefault(), _currentUser.ClientId!);
      if (oStore == null)
      {
        serviceResult.CreateError("Store", new string[] { "Store not found" });
        return serviceResult;
        //sale channel is not found
      }
      var setting = await _clientRepository.GetGenericSettingByClientIdAsync(_currentUser.ClientId!);
      bool show3PlInfo = false;
      if (setting is not null && !string.IsNullOrEmpty(setting.SettingConfig!))
      {
        string s3pl = UtilityHelper.GetClientSettingValueWithByKey(setting.SettingConfig!, "order", "show3PlInfo");
        show3PlInfo = UtilityHelper.GetBoolFromString(s3pl);
      }
        var result =
      new
      {
        SalePersonId = _currentUser.EmployeeIdStr,
        SaleChannelConfigId = oSaleChannelConfig!.SaleChannelConfigId,
        DefaultProductStationId = client.DefaultProductStationId,
        Store = new
        {
          storeId = oStore!.StoreId,
          storeName = oStore!.StoreName,
        },
        others = new
        {
          show3PlInfo = show3PlInfo
        }
      };
      serviceResult = new ServiceResultDTO(result);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
