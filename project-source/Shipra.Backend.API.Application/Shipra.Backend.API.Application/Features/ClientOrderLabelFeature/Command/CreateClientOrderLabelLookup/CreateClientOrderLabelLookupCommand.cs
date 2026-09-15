using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderLabelUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.CreateClientOrderLabelLookup;
public class CreateClientOrderLabelLookupCommand : IRequest<ServiceResultDTO>
{
  public CreateOrderLabelModel? model { get; set; }
}
public class CreateClientOrderLabelLookupCommandHandler : RequestHandlerBase<CreateClientOrderLabelLookupCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public CreateClientOrderLabelLookupCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateClientOrderLabelLookupCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientOrderLabelLookupCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      // Get all existing labels from DB
      var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelLookupForSelection(_currentUser.ClientId!);

      var oClientOrderLabelLookup = allClientOrderLabels.FirstOrDefault(x => x.LabelName == request!.model!.Label);
      if (oClientOrderLabelLookup != null)
      {
        // return error message
        serviceResult.CreateError("AlreadyExist", new string[] { $"Label with name: {request!.model!.Label} already exist." });
        return serviceResult;
      }

      oClientOrderLabelLookup = ClientOrderLabelLookup.Create(_currentUser.ClientId!, request!.model!.Label, request!.model!.ColorCode, _currentUser.EmployeeId!);
      await _orderRepository.CreateClientOrderLabelLookup(oClientOrderLabelLookup);
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oClientOrderLabelLookup.ClientOrderLabelLookupId, Message = "Labels added successfully" });

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class CreateClientOrderLabelLookupValidator : AbstractValidator<CreateClientOrderLabelLookupCommand>
{
  public CreateClientOrderLabelLookupValidator()
  {
    RuleFor(v => v.model)
            .NotNull().WithMessage("Model cannot be null.")
            .DependentRules(() =>
            {
              RuleFor(v => v.model!.Label)
                  .NotEmpty().WithMessage("Label is required.");
            });
  }
}


