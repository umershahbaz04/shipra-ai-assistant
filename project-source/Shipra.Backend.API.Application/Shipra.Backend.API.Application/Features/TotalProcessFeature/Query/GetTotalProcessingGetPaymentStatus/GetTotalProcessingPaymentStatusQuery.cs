using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.TotalProcessFeature.Query.GetTotalProcessingGetPaymentStatus;
public class GetTotalProcessingPaymentStatusQuery : IRequest<ServiceResultDTO>
{
  public string? CheckoutId { get; set; }
}
public class GetTotalProcessingPaymentStatusQueryHandler : RequestHandlerBase<GetTotalProcessingPaymentStatusQuery, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedTotalProcessingRepository _sharedTotalProcessingRepository;

  public GetTotalProcessingPaymentStatusQueryHandler(IConfigRepository configRepository, ISharedTotalProcessingRepository sharedTotalProcessingRepository, IServiceProvider serviceProvider, ILogger<GetTotalProcessingPaymentStatusQueryHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedTotalProcessingRepository = sharedTotalProcessingRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetTotalProcessingPaymentStatusQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }

      string requestResponse = await _sharedTotalProcessingRepository.GetCheckoutPaymentDetail(request.CheckoutId,_currentUser.ClientIdStr, mcconfig.Value!);
      if (!string.IsNullOrEmpty(requestResponse))
      {
        var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<PaymentDetailResponseModel>>(requestResponse);
        if (result != null && result!.isSuccess)
        {
          //Success: The Order Address to get customer information

          serviceResult = new ServiceResultDTO(result.result!);

        }
        else
        {
          serviceResult.Errors = result?.errors;
          serviceResult.IsSuccess = false;
        }
      }
      else
      {
        serviceResult.IsSuccess = false;
        serviceResult.Errors!.Add("ThirdPartyError", new string[] { "Error while create link" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetTotalProcessingGetPaymentStatusQueryValidator : AbstractValidator<GetTotalProcessingPaymentStatusQuery>
{
  public GetTotalProcessingGetPaymentStatusQueryValidator()
  {
    RuleFor(x => x.CheckoutId).NotEmpty().NotNull();
  }
}
