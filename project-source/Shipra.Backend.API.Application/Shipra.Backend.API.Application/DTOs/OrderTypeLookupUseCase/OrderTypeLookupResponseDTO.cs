using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.OrderTypeLookupUseCase;
internal class OrderTypeLookupResponseDTO
{
}

public class OrderTypeLookupResponseDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}

public class OrderTypeLookupResponseModel
{
  public int OrderTypeId { get; set; }
  public string? OrderTypeName { get; set; }
}
