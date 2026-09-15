using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClient;
public class UpdateClientCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? ClientName { get; set; }
  public string? ClientImage { get; set; } 
  public string? ClientCompanyName { get; set; }
  public string? Mobile { get; set; }
  public string? Phone { get; set; }
  public string? LicenseNo { get; set; } 
  public int RegionTimeZoneId { get; set; }
  public ClientAddressRequestModel? ClientAddress { get; set; }

}
