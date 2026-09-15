using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DisableProductStock;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.EnableProductStock;
public class EnableProductStockCommand : IRequest<ServiceResultDTO>
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
