using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Request;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClient;
public class CreateClientCommand : CommandBase, IRequest<ServiceResultDTO>
{
  public string? ClientName { get; set; }
  public string? ClientImage { get; set; }
  //public int? CountryId { get; set; }
  //public int? RegionId { get; set; }
  //public int? CityId { get; set; }
  public string? ClientCompanyName { get; set; }
  public string? Email { get; set; }
  public bool? IsPreverifyEmail { get; set; }
  public string? UserName { get; set; }
  public string? Mobile { get; set; }
  //public string? StreetAddress { get; set; }
  //public string? Zip { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? Password { get; set; }
  public string? Phone { get; set; }
  public ClientAddressRequestModel? ClientAddress { get; set; }

}

public class CreateTenantRequestModel
{
  public string? Email { get; set; }
  public string? UserName { get; set; }
  public string? Phone { get; set; }
  public string? Address { get; set; }
  public string? Password { get; set; }
  public string? ConfirmPassword { get; set; }
}
