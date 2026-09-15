using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.WipeOutClientData;

public class WipeOutClientDataCommandValidator : AbstractValidator<WipeOutClientDataCommand>
{
    public WipeOutClientDataCommandValidator()
    {
        // No fields to validate in the command since the ID is retrieved from the JWT token
    }
}
