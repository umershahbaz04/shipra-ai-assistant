using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.BatchOutScanDeliveryTask;
public class CreateDeliveryTaskAndAddToExistingNoteCommandValidator : AbstractValidator<CreateDeliveryTaskAndAddToExistingNoteCommand>
{
  public CreateDeliveryTaskAndAddToExistingNoteCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
    RuleFor(v => v.AssigningDate).NotNull().NotEmpty();
    RuleFor(v => v.DriverId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);

    When(v => v.AddToExisting.GetValueOrDefault() == true, () =>
    {
      RuleFor(v => v.NoteNo).NotEmpty().NotNull(); 
    });
  }
}
