using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MediatR;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.ValidatedOrderAddressForCarrier;
public class ValidateOrderAddressForCarrierQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
  public List<OrderAddressValidateAgainstCarrierWithNameModel>? OrderAddress { get; set; }
}
public class ValidateOrderAddressForCarrierQueryHandler : RequestHandlerBase<ValidateOrderAddressForCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly ICountryRepository _countryRepository;

  public ValidateOrderAddressForCarrierQueryHandler(ICarrierRepository carrierRepository, ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<ValidateOrderAddressForCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ValidateOrderAddressForCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      List<Country> countries = await _countryRepository.GetAllCacheCountries();
      List<CivilEntityExtended> civilEntityCarrierMapped = await _countryRepository.GetAllCacheCivilEntityCarrierMapped();
      List<CivilEntityExtended> filterdCivilEntityCarrier = civilEntityCarrierMapped.Where(x => x.CarrierId == request.CarrierId).ToList();

      List<OrderValidationForCarrierDispatchExSideResult> validationResults = new List<OrderValidationForCarrierDispatchExSideResult>();
      var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId);
      if (carrier is null)
      {
        //throw error
      }
      CarrierLocation carrierLocation = await _carrierRepository.GetCarrierLocationByCarrier(carrier!);

      foreach (var oAddress in request.OrderAddress!)
      {
        Dictionary<string, bool> validationResultsEntity = new();

        var country = countries.FirstOrDefault(x => x.Name == oAddress.Country);
        if (country is null) continue;

        Dictionary<string, List<CivilEntityExtended>?> civilEntityConfigWithKeysData = new(); 
        List<string> keyList = OrderCommon.ExtractCarrierLocationKeys(carrierLocation.AddressingScheme!);
        if (keyList.Count == 0)
        {
          var counrtyKeyList = JsonConvert.DeserializeObject<CountryAddressSchema>(country.AddressingScheme!);
          keyList = counrtyKeyList!.keys!;
        } 
        Dictionary<string, string?> countryConfigKeys = OrderCommon.GetPropertyValues(oAddress, keyList!);
        var orderValidationResult = new OrderValidationAddressResult { Country = country.CountryId };

        foreach (var keyValue in countryConfigKeys)
        {
          string propertyName = keyValue.Key;
          string? propertyValue = keyValue.Value;
          string currentKeyParent = OrderCommon.GetParentKey(country.AddressingScheme!, propertyName!)!;
          string requiredParentKey = OrderCommon.GetRequiredParentKey(country.AddressingScheme!, propertyName);

          OrderCommon.AddCivilEntityData(requiredParentKey, country.CountryId, civilEntityCarrierMapped, civilEntityConfigWithKeysData);

          var typeId = CivilEntityHelper.GetCivilEntityTypeIdFromKey(propertyName);
          var parentIds = OrderCommon.GetCivilEntityValueByKey(civilEntityConfigWithKeysData, currentKeyParent)
                          ?.Select(c => c.CivilEntityExtendedId)
                          .ToList() ?? new List<long>();

          if (!carrier!.IsDispatchExCompany.GetValueOrDefault())
          {
            // When second value > 0, check against CivilEntityExtendedId
            CivilEntityExtended? matchedEntity = filterdCivilEntityCarrier.FirstOrDefault(entity =>
                                  entity.CivilEntityName?.Trim().ToLower() == propertyValue?.Trim().ToLower() &&
                                  entity.CivilEntityTypeId == typeId &&
                                  parentIds.Contains(entity.ParentId ?? -1));


            if (!string.IsNullOrEmpty(carrierLocation.AddressingScheme))
            {
              var selectedIds = string.Join(",", parentIds);

              int carrierId = 0;
              if (carrier is not null && carrier.ValidateAddress.GetValueOrDefault())
              {
                carrierId = request.CarrierId;
              }
              List<AddressCommonLookupModel> data = await _countryRepository.GetAddressEntitiesByType(
                selectedIds, requiredParentKey, propertyName, country.CountryId, request.CarrierId);
              OrderCommon.AddAddressEntities(propertyName, data, civilEntityCarrierMapped, civilEntityConfigWithKeysData);
              bool isCountryRequired = CivilEntityHelper.CheckRequiredKeyInCountryAddressingShceck(country.AddressingScheme!, propertyName);
              bool isCarrierRequired = OrderCommon.IsKeyRequired(carrierLocation.AddressingScheme, propertyName);

              
              if ((isCountryRequired || isCarrierRequired) && matchedEntity is null)
              {
                if (propertyName == "streetAddress" && !string.IsNullOrEmpty(oAddress.StreetAddress))
                {
                  OrderCommon.SetProperty(orderValidationResult, propertyName, oAddress.StreetAddress);
                }
                else
                {
                  validationResultsEntity[propertyName] = true;
                }
              }
              else if (matchedEntity != null)
              {
                OrderCommon.SetProperty(orderValidationResult, propertyName, $"{matchedEntity.MappedId}_{matchedEntity.CivilEntityExtendedId}");
              }
            }
          }
        }
        bool isValidAddress = validationResultsEntity.Keys.Count == 0;

        validationResults.Add(new OrderValidationForCarrierDispatchExSideResult
        {
          OrderNo = oAddress.OrderNo,
          IsValidAddress = isValidAddress,
          InvalidProperties = validationResultsEntity.Keys.ToList(),
          InvalidPropertiesWithData = validationResultsEntity,
          Address = orderValidationResult
        });
      }

      // Now `validationResults` contains the list of validated orders

      serviceResult = new ServiceResultDTO(validationResults);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public class OrderValidationForCarrierDispatchExSideResult
  {
    public string? OrderNo { get; set; }
    public bool IsValidAddress { get; set; }
    public List<string> InvalidProperties { get; set; } = new List<string>();
    public Dictionary<string, bool> InvalidPropertiesWithData = new();
    public OrderValidationAddressResult? Address { get; set; }
  }
  public class OrderValidationAddressResult
  {
    public int? Country { get; set; }
    public string? City { get; set; }
    public string? Area { get; set; }
    public string? Province { get; set; }
    public string? PinCode { get; set; }
    public string? State { get; set; }
    public string? StreetAddress { get; set; }
  }
}
