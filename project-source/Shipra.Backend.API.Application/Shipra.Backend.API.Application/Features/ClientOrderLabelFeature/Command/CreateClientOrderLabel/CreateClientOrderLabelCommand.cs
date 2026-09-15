using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderLabelUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.CreateClientOrderLabel;
public class CreateClientOrderLabelCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public List<CreateOrderLabelModel>? Labels { get; set; }
}

public class CreateClientOrderLabelCommandHandler : RequestHandlerBase<CreateClientOrderLabelCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public CreateClientOrderLabelCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateClientOrderLabelCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientOrderLabelCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var ordersList = await _orderRepository.GetOrdersByOrderNos(request.OrderNos!, _currentUser.ClientId!);
      var invalidOrder = ordersList.Where(x => x.TrackingLock.GetValueOrDefault()).ToList();
      var validOrders = ordersList.Where(order => !order.TrackingLock.GetValueOrDefault()).ToList();
      foreach (var order in validOrders)
      {
        // Get all existing labels from DB
        var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelByOrderId(_currentUser.ClientId!, order.OrderId!);
        var existingLabels = allClientOrderLabels.ToList(); // List<ClientOrderLabel>

        // Get labels from request (comma separated)
        // Step 2: Request values
        var requestedLabels = request.Labels!.Where(x => !string.IsNullOrWhiteSpace(x.Label))
            .Select(x => new
            {
              Label = x.Label!.Trim(),
              ColorCode = x.ColorCode
            }).DistinctBy(x => x.Label, StringComparer.OrdinalIgnoreCase).ToList();

        // Identify labels to remove (in DB but not in request)
        var labelsToRemove = existingLabels
      .Where(dbLabel => !requestedLabels.Any(req => string.Equals(req.Label, dbLabel.LabelName, StringComparison.OrdinalIgnoreCase))).ToList();

        var existingLabelNames = existingLabels.Select(x => x.LabelName!).ToList();
        // Identify labels to add (in request but not in DB)
        var labelsToAdd = requestedLabels
         .Where(req => !existingLabelNames.Any(existing => string.Equals(existing, req.Label, StringComparison.OrdinalIgnoreCase))).ToList();

        foreach (var label in labelsToRemove)
        {
          // You can either soft-delete or hard-delete based on your design
          await _orderRepository.DeleteClientOrderLabel(label); // Implement accordingly
        }


        foreach (var labels in labelsToAdd)
        {
          var newLabel = ClientOrderLabel.Create(_currentUser.ClientId!, order.OrderId!, labels.Label, labels.ColorCode, _currentUser.EmployeeId!);
          await _orderRepository.CreateClientOrderLabel(newLabel);
        }
        #region update order
        var labelsToAddCsv = string.Join(",", requestedLabels.Select(x => x.Label));
        order.UpdateOrderLabel(labelsToAddCsv, _currentUser.EmployeeId!);
        await _orderRepository.UpdateOrder(order);
        #endregion
  
      }
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = "Labels added successfully" });

      return serviceResult;
    }

    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

public class CreateClientOrderLabelCommandValidator : AbstractValidator<CreateClientOrderLabelCommand>
{
  public CreateClientOrderLabelCommandValidator()
  {
    RuleFor(x => x.OrderNos).NotNull().NotEmpty();
    RuleFor(x => x.Labels).Must(x => x != null).WithMessage("OrderItems list must contain at least one item.");
    RuleForEach(x => x.Labels).SetValidator(x => new CreateOrderLabelItemValidator());

  }
}
public class CreateOrderLabelItemValidator : AbstractValidator<CreateOrderLabelModel>
{
  public CreateOrderLabelItemValidator()
  {
    RuleFor(v => v.Label).NotNull().NotEmpty();
  }
}

