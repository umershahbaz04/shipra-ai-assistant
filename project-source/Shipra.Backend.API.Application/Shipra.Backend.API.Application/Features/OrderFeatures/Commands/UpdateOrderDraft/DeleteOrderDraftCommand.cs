using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderDraft;
public class DeleteOrderDraftCommand : IRequest<ServiceResultDTO>
{
  public long OrderDraftId { get; set; } 
}
public class DeleteOrderDraftCommandHandler : RequestHandlerBase<DeleteOrderDraftCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public DeleteOrderDraftCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DeleteOrderDraftCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteOrderDraftCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orderDraft = await _orderRepository.GetDraftOrderById(request.OrderDraftId, _currentUser.ClientId!);
      if (orderDraft is null)
      {
        throw new EntityNotFoundException("OrderDraft", request.OrderDraftId!);
      }
      var isDeleted = await _orderRepository.DeleteDraftOrder(orderDraft);

      if (isDeleted)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = "Deleted Successfully"
        });
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
public class DeleteOrderDraftCommandValidator : AbstractValidator<DeleteOrderDraftCommand>
{
  public DeleteOrderDraftCommandValidator()
  {
    RuleFor(x => x.OrderDraftId).NotNull().NotEmpty().GreaterThan(0);
  }
}
