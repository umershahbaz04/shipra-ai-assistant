using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.PaymentMethodLookupUseCase;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using MediatR;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.PaymentMethodLookupFeatures.Query.GetAllPaymentMethodLookupQuery;
public class GetAllPaymentMethodLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>>>
{
}

public class GetAllPaymentMethodLookupQueryHandler : RequestHandlerBase<GetAllPaymentMethodLookupQuery, ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>>>
{
  private readonly IPaymentMethodLookupRepository _paymentMethodLookupRepository;

  public GetAllPaymentMethodLookupQueryHandler(IPaymentMethodLookupRepository PaymentMethodLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllPaymentMethodLookupQueryHandler> logger) : base(serviceProvider, logger)
  {
    _paymentMethodLookupRepository = PaymentMethodLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>>> HandleRequest(GetAllPaymentMethodLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>> serviceResult = new ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>>();

    try
    {

      var data = await _paymentMethodLookupRepository.GetAllPaymentMethodLookup();
      var obj = PaymentMethodLookup.AddDefault();
      data?.Add(obj);
      var responseDto = data!.Select(x => new PaymentMethodLookupResponseModel()
      { Code = x.Code, PaymentMethodId = x.PaymentMethodId, PMName = x.PMName }).OrderBy(x => x.PaymentMethodId).ToList();

      serviceResult = new ServiceResultDTOWithTypeModel<List<PaymentMethodLookupResponseModel>>(responseDto);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
