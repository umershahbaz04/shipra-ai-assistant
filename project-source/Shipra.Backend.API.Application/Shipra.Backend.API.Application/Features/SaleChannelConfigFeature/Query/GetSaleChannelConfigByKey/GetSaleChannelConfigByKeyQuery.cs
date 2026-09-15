using FluentValidation;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigByKey;
public class GetSaleChannelConfigByKeyQuery : IRequest<ServiceResultDTO>
{
  public string? SaleChannelKey { get; set; }
  public string? ClientId { get; set; }
  public string? SecretKey { get; set; }
}
public class GetSaleChannelConfigByKeyQueryValidator : AbstractValidator<GetSaleChannelConfigByKeyQuery>
{
  public GetSaleChannelConfigByKeyQueryValidator()
  {
    RuleFor(v => v.SaleChannelKey).NotNull().NotEmpty();
    RuleFor(v => v.ClientId).NotNull().NotEmpty();
    RuleFor(v => v.SecretKey).NotNull().NotEmpty();
  }
}
