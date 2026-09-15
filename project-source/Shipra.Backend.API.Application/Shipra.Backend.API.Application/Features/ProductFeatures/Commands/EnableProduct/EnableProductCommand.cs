using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.EnableProduct;
public class EnableProductCommand : IRequest<ServiceResultDTO>
{
  public string? ProductId { get; set; }
}
public class EnableProductCommandValidator : AbstractValidator<EnableProductCommand>
{
  public EnableProductCommandValidator()
  {
    RuleFor(v => v.ProductId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
