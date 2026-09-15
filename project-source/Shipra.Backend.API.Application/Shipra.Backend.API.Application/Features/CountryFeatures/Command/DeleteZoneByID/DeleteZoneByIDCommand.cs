using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Command.DeleteZoneByID;

public class DeleteZoneByIDCommand : IRequest<ServiceResultDTO>
{
  public int ZoneID { get; set; }
}

public class DeleteZoneByIDCommandHandler : RequestHandlerBase<DeleteZoneByIDCommand, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public DeleteZoneByIDCommandHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<DeleteZoneByIDCommand> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteZoneByIDCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      bool res = await _countryRepository.DeleteZoneByID(request.ZoneID);
      var responseObj = new
      {
        result = res,
        errMsg = res ? "" : "Error while deleting zone."
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
