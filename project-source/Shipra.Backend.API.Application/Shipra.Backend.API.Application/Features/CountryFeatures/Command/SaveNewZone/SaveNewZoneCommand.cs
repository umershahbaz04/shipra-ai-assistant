using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Command.SaveNewZone;

public class SaveNewZoneCommand : IRequest<ServiceResultDTO>
{
  public int ZoneId { get; set; }
  public string? Code { get; set; }
  public string? Name { get; set; }
  public string? NameArabic { get; set; }
  public int? CityID { get; set; }
  public string? Coords { get; set; }
}

public class SaveNewZoneCommandHandler : RequestHandlerBase<SaveNewZoneCommand, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public SaveNewZoneCommandHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<SaveNewZoneCommand> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaveNewZoneCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Zone zone = new Zone(request.Name ?? "", request.CityID, request.Coords, request.Code, request.NameArabic);
      if (request.ZoneId > 0)
      {
        var existing = await _countryRepository.GetZoneById(request.ZoneId);
        if (existing != null)
        {
          existing.UpdateZone(request.Name ?? "", request.CityID, request.Coords, request.Code, request.NameArabic);
          zone = existing;
        }
      }

      bool res = await _countryRepository.SaveNewZone(zone);
      var responseObj = new
      {
        Result = res,
        ErrorMsg = res ? "" : "Failed to save zone information."
      };

      serviceResult = new ServiceResultDTO(responseObj);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
