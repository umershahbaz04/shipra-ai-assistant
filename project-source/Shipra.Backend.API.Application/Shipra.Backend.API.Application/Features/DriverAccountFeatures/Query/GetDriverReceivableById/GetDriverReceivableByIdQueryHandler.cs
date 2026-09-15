using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DriverExpenseUseCase;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverReceivableById;
public class GetDriverReceivableByIdQueryHandler : RequestHandlerBase<GetDriverReceivableByIdQuery, ServiceResultDTOWithTypeModel<DriverReceivableResponseModel>>
{
  private readonly IDriverAccountRepository _driverAccount;

  public GetDriverReceivableByIdQueryHandler(IDriverAccountRepository driverAccount, IServiceProvider serviceProvider, ILogger<GetDriverReceivableByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<DriverReceivableResponseModel>> HandleRequest(GetDriverReceivableByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<DriverReceivableResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<DriverReceivableResponseModel>();
    try
    {
      if (!GuidHelper.Validator(request.DriverReceivableId!))
      {
        throw new InvalidIdTypeException($"{nameof(request.DriverReceivableId)} {request!.DriverReceivableId}");
      }

      var driverRecivable = await _driverAccount.GetDriverReceivableById(new DriverReceivableId(new Guid(request.DriverReceivableId!)));
      if (driverRecivable is null)
      {
        throw new EntityNotFoundException("DriverReceivable", request.DriverReceivableId!);
      }

      var mapper = _mapper.Map<DriverReceivableResponseModel>(driverRecivable);
      serviceResult = new ServiceResultDTOWithTypeModel<DriverReceivableResponseModel>(mapper);
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

