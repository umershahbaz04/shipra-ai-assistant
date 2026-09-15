using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateOrderStatusByDriver;
public class UpdateOrderStatusByDriverCommandValidator : AbstractValidator<UpdateOrderStatusByDriverCommand>
{
  public UpdateOrderStatusByDriverCommandValidator()
  {
    RuleFor(v => v.DeliveryNoteDetailId).NotNull().NotEmpty();
    RuleFor(v => v.CarrierTrackingStatusId).NotNull().NotEmpty();

  }
}

