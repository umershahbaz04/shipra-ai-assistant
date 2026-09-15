using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.CreateCarrier;
public class CreateCarrierCommand : IRequest<ServiceResultDTO>
{
        public string? Name { get; set; }
        public string? CarrierImage { get; set; }
        public string? CarrierWebsite { get; set; }
        public string? Config { get; set; }
        public string? InputRequiredConfig { get; set; }
        public int? CountryId { get; set; }
}
public class CreateCarrierCommandHandler : RequestHandlerBase<CreateCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public CreateCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<CreateCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCarrierCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {

      //List<Carrier> allClientCarriers = await _carrierRepository.GetAllCarrierWithoutClientCarrier(_currentUser.ClientId);  
      //var carrierId = Carrier.GetNextCarrierId(allClientCarriers.LastOrDefault());

      //Carrier carrier = Carrier.CreateCarrier(carrierId, request.Name, request.CarrierImage, request.CarrierWebsite, request.Config, request.InputRequiredConfig, request.CountryId, _currentUser.EmployeeId!);

      //var oCarrier = await _carrierRepository.CreateCarrier(carrier) as Carrier;
      //if (oCarrier is not null)
      //{
      //  var result = new BaseResponseDto()
      //  {
      //    Data = oCarrier.CarrierId,
      //    Message = NotificationConstants.Success
      //  };
      //  serviceResult = new ServiceResultDTO(result);
      //  serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      //  return serviceResult;
      //}
      //else
      //{
      //  serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
      //  return serviceResult;
      //}
      await Task.Delay(1);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class CreateCarrierCommandValidator : AbstractValidator<CreateCarrierCommand>
{
  public CreateCarrierCommandValidator()
  {
    RuleFor(v => v.Name).NotNull().NotEmpty();
    RuleFor(v => v.CarrierWebsite).NotNull().NotEmpty();
    RuleFor(v => v.CarrierImage).NotNull().NotEmpty();
    RuleFor(v => v.CountryId).NotNull().NotEmpty().GreaterThan(0);
  }
}
