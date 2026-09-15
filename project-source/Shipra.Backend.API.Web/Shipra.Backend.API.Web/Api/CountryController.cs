using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllDeliveryService;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelDownloadCountry;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.ExcelExportAddressEntitiesByType;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAddressEntitiesByType;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCityByNameForSelection;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCivilEntityType;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCountry;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAreaByZoneIdQuery;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetBlocksByCityId;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCities;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCitiesByCountryId;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCitiesByProvinceAndCountryId;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCityByRegionIdQuery;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCivilEntityIdByLatitudeAndLongitude;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCountryCityRegionIdByName;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetProvinceByCountryId;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetStates;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetStatesByCountryId;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllZonesWithCoords;
using Shipra.Backend.API.Application.Features.CountryFeatures.Command.SaveNewZone;
using Shipra.Backend.API.Application.Features.CountryFeatures.Command.UpdateZoneFromZoneBoundries;
using Shipra.Backend.API.Application.Features.CountryFeatures.Command.DeleteZoneByID;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCities;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Web.Api;
public class CountryController : BaseApiController
{
  public CountryController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  //[HttpPost("CreateCountry")]
  //public async Task<ActionResult> CreateCountry([FromBody] CreateCountryCommand request, CancellationToken cancellationToken = default)
  //{
  //  var response = await Mediator.Send(request, cancellationToken);
  //  return Ok(response);

  //}

  //[HttpPost("UpdateCountry")]
  //public async Task<ActionResult> UpdateCountry([FromBody] UpdateCountryCommand request, CancellationToken cancellationToken = default)
  //{
  //  var response = await Mediator.Send(request, cancellationToken);
  //  return Ok(response);
  //}
  #endregion
  #region query
  [HttpGet("GetAllCountry")]
  public async Task<ActionResult> GetAllCountries()
  {
    var request = new GetAllCountryQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
  [HttpPost("GetAllCityByNameForSelection")]
  public async Task<ActionResult> GetAllCityByNameForSelection([FromBody] GetAllCityByNameForSelectionQuery request)
  {
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }

  [HttpGet("GetAllCivilEntityType")]
  public async Task<ActionResult> GetAllCivilEntityType(CancellationToken cancellationToken = default)
  {
    GetAllCivilEntityTypeQuery request = new GetAllCivilEntityTypeQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }


  [HttpGet("GetCityByRegionId")]
  public async Task<ActionResult> GetCityByRegionId([FromQuery] GetCityByRegionIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCitiesByCountryId")]
  public async Task<ActionResult> GetCitiesByCountryId([FromQuery] GetCitiesByCountryIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCities")]
  public async Task<ActionResult> GetCities([FromQuery] GetCitiesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCitiesByProvinceAndCountryId")]
  public async Task<ActionResult> GetCitiesByProvinceAndCountryId([FromQuery] GetCitiesByProvinceAndCountryIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCitiesByStateAndCountryId")]
  public async Task<ActionResult> GetCitiesByStateAndCountryId([FromQuery] GetCitiesByStateAndCountryIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetCountryRegionCityIdByName")]
  public async Task<ActionResult> GetCountryRegionCityIdByName([FromBody] GetCountryCityAreaIdByNameQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetPinCodesByCityId")]
  public async Task<ActionResult> GetPinCodesByCityId([FromQuery] GetPinCodesByCityIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetProvinceByCountryId")]
  public async Task<ActionResult> GetProvinceByCountryId([FromQuery] GetProvinceByCountryIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetStatesByCountryId")]
  public async Task<ActionResult> GetStatesByCountryId([FromQuery] GetStatesByCountryIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAreasByCityId")]
  public async Task<ActionResult> GetAreasByCityId([FromQuery] GetAreasByCityIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAddressEntitiesByType")]
  public async Task<ActionResult> GetAddressEntitiesByType([FromQuery] GetAddressEntitiesByTypeQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("ExcelExportAddressEntitiesByType")]
  public async Task<ActionResult> ExcelExportAddressEntitiesByType([FromQuery] ExcelExportAddressEntitiesByTypeQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName(!string.IsNullOrEmpty(request!.NextEntityType!) ? request!.NextEntityType! : "Address Entity" ));
  }

  [HttpGet("ExcelExportAllAreas")]
  public async Task<ActionResult> ExcelExportAllCity(CancellationToken cancellationToken = default)
  {
    var request = new Application.Features.CountryFeatures.Query.ExcelDownloadCity.ExcelExportAllAreasQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("City"));
  }
  [HttpGet("ExcelExportAllCountry")]
  public async Task<ActionResult> ExcelExportAllCountry(CancellationToken cancellationToken = default)
  {
    ExcelExportAllCountryQuery request = new ExcelExportAllCountryQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Country"));
  }
  [HttpGet("ExcelExportCities")]
  public async Task<ActionResult> ExcelExportAllRegion(CancellationToken cancellationToken = default)
  {
    var request = new Application.Features.CountryFeatures.Query.ExcelDownloadRegion.ExcelExportAllCityQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Region"));
  }
   
  [HttpPost("GetCivilEntityIdByLatitudeAndLongitude")]
  public async Task<ActionResult> GetCivilEntityIdByLatitudeAndLongitude([FromBody] GetCivilEntityIdByLatitudeAndLongitudeQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllZonesWithCoords")]
  public async Task<ActionResult> GetAllZonesWithCoords(CancellationToken cancellationToken = default)
  {
    var request = new GetAllZonesWithCoordsQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaveNewZone")]
  public async Task<ActionResult> SaveNewZone([FromBody] SaveNewZoneCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateZoneFromZoneBoundries")]
  public async Task<ActionResult> UpdateZoneFromZoneBoundries([FromQuery] string zoneName, [FromQuery] int zoneID, [FromQuery] int cityID, CancellationToken cancellationToken = default)
  {
    var request = new UpdateZoneFromZoneBoundriesCommand
    {
      ZoneName = zoneName,
      ZoneID = zoneID,
      CityID = cityID
    };
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeleteZoneByID")]
  public async Task<ActionResult> DeleteZoneByID([FromQuery] int zoneID, CancellationToken cancellationToken = default)
  {
    var request = new DeleteZoneByIDCommand { ZoneID = zoneID };
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion
}

