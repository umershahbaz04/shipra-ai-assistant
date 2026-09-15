using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteMyCarrierRetunReport;
public class DeleteMyCarrierReturnReportCommandValidator : AbstractValidator<DeleteMyCarrierReturnReportCommand>
{
  public DeleteMyCarrierReturnReportCommandValidator()
  {
    RuleFor(v => v.CarrierRRId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
