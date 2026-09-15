using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DisableProductStock;
public class DisableProductStockCommand : IRequest<ServiceResultDTO>
{
  public int? ProductStockId { get; set; }
}

public class DisableProductStockCommandValidator : AbstractValidator<DisableProductStockCommand>
{
  public DisableProductStockCommandValidator()
  {
    RuleFor(v => v.ProductStockId).NotNull().NotEmpty();
  }
}
