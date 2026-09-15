using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Command.UpdateProductCategory;
public class UpdateProductCategoryCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int ProductCategoryId { get;  set; }
  public string? CategoryName { get; set; }
}
