using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.StationLookupUserCase;
public class StationLookupResponseDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}

public class StationLookupResponseModel
{
  public int ProductStationId { get; set; }
  public string? Sname { get; set; }
}
