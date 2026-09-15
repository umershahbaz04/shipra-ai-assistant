using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.DeleteOrderNote;
public class DeleteOrderNoteCommandValidator : AbstractValidator<DeleteOrderNoteCommand>
{
  public DeleteOrderNoteCommandValidator()
  {
    RuleFor(v => v.OrderNoteId).NotNull().NotEmpty();
  }
}
