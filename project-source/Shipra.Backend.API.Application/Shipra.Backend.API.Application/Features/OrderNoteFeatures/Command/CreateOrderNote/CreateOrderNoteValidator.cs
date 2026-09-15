using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.CreateOrderNote;
public class CreateOrderNoteValidator : AbstractValidator<CreateOrderNoteCommand>
{
  public CreateOrderNoteValidator()
  {
    RuleFor(v => v.NoteDescription).NotEmpty().NotEmpty();
  }
}

