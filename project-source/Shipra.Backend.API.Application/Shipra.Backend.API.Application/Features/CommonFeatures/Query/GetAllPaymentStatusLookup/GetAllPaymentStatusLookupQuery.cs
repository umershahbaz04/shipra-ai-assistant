using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Application.Common;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Core.CommonAggregate;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllPaymentStatusLookupQuery;
public class GetAllPaymentStatusLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{

}
public class GetAllPaymentStatusLookupQueryHandler : RequestHandlerBase<GetAllPaymentStatusLookupQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;

  public GetAllPaymentStatusLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger
    <GetAllPaymentStatusLookupQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllPaymentStatusLookupQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var expense = await _commonLookupRepository.GetAllPaymentStatusLookup();
      PaymentStatusLookup obj = PaymentStatusLookup.AddDefault();
      expense?.Add(obj);
      var data = expense!.Select(x => new CommonLookupResponseModel()
      {
        id = x.PaymentStatusId,
        text = x.StatusName
      }).OrderBy(x => x.id).ToList();
      response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>(data);
      response.CreateSuccessResponse();
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
