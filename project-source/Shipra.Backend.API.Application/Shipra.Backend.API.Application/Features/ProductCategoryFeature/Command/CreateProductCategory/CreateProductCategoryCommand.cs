using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.CreateProductCategory;
public class CreateProductCategoryCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? CategoryName { get; set; }
}
