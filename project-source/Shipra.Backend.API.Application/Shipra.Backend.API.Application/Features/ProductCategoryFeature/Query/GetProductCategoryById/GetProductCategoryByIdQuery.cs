using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductCategoryUseCase;

namespace Shipra.Backend.API.Application.Features.ProductCategoryFeature.Query.GetProductCategoryById;
public class GetProductCategoryByIdQuery: IRequest<ServiceResultDTOWithTypeModel<ProductCategoryResponseModel>>
{
  public int ProductCategoryId { get; set; }
}
