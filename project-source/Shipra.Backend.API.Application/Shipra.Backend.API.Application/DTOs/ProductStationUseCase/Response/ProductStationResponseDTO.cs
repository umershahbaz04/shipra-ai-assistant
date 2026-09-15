using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;
public class ProductStationDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}
public class ProductStationResposeModel
{
  public int ProductStationId { get; set; }
  public string? StationCode { get; set; }
  public string? Name { get; set; } 
  public bool? Active { get; set; }
  public bool? IsDeleted { get; set; }
  public bool? IsDefault { get; set; }
  public DateTime? CreatedOn { get; set; } 
}
