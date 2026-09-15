using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.DisableProduct;
public class DisableProductCommand : IRequest<ServiceResultDTO>
{
  public string? ProductId { get; set; }
}
public class DisableProductCommandValidator : AbstractValidator<DisableProductCommand>
{
  public DisableProductCommandValidator()
  {
    RuleFor(v => v.ProductId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
