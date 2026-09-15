using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateMyCarrierReturnReport;
public class CreateMyCarrierReturnReportCommandValidator : AbstractValidator<CreateMyCarrierReturnReportCommand>
{
  public CreateMyCarrierReturnReportCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
  }
}
