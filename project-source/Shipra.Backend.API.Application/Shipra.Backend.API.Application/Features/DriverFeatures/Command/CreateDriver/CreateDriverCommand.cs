using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.CreateDriver;
public class CreateDriverCommand : IRequest<ServiceResultDTO>
{
  public string? EmployeeName { get; set; }
  public int? GenderId { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string? EmployeeImage { get; set; }
  public int? CountryId { get; set; }
  public int? RegionId { get; set; }
  public int? CityId { get; set; }
  public string? WorkEmail { get; set; }
  public bool? IsPreverifyEmail { get; set; }
  public string? UserName { get; set; }
  public string? Mobile { get; set; }
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? Zip { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? Password { get; set; }
  public string? Phone { get; set; }
  public int? UserRoleId { get; set; }
}
