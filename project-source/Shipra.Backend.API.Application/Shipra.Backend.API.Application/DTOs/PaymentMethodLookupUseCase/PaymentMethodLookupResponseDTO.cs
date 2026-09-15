using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.DTOs.PaymentMethodLookupUseCase;

public class PaymentMethodLookupResponseDTO<T> : ErrorResponse
{
  public T? Result { get; set; }
}

public class PaymentMethodLookupResponseModel
{
  public int PaymentMethodId { get; set; }
  public string? Code { get; set; }
  public string? PMName { get; set; }
}
