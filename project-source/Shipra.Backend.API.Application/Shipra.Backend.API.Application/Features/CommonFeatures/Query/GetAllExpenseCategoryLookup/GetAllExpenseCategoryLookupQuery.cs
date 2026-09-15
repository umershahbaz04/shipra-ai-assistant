using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllExpenseCategoryQuery;
public class GetAllExpenseCategoryLookupQuery : IRequest<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
}
public class GetAllExpenseCategoryLookupQueryHandler : RequestHandlerBase<GetAllExpenseCategoryLookupQuery, ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>>
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  public GetAllExpenseCategoryLookupQueryHandler(ICommonLookupRepository commonLookupRepository, IServiceProvider serviceProvider, ILogger<GetAllExpenseCategoryLookupQuery> logger) : base(serviceProvider, logger)
  {
    _commonLookupRepository = commonLookupRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>> HandleRequest(GetAllExpenseCategoryLookupQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<List<CommonLookupResponseModel>>();
    try
    {
      var expense = await _commonLookupRepository.GetAllExpenseCategories(_currentUser.ClientId!);
      var data = expense!.Select(x => new CommonLookupResponseModel
      {
        id = x.ExpenseCategoryId,
        text = x.ExpenceName
      }).ToList();
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
