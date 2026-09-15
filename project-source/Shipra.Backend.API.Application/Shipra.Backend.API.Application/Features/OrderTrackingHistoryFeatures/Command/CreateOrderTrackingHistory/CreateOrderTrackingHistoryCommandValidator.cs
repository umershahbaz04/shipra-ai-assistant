using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Command.CreateOrderTrackingHistory;
public class CreateOrderTrackingHistoryCommandValidator : AbstractValidator
  <CreateOrderTrackingHistoryCommand>
{
  public CreateOrderTrackingHistoryCommandValidator()
  {
    RuleFor(v => v.TrackingStatusComments).NotEmpty().NotEmpty();
  }
}
