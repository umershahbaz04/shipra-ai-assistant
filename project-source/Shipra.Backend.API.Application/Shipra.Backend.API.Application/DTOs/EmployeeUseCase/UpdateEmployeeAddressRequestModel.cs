using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
public class UpdateEmployeeAddressRequestModel : AddressRequestDTO
{
  public string? AddressId { get; set; }
}
