using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.StoresUseCase.Responses;
namespace Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoreByIdQuery;
public class GetStoreByIdQuery : IRequest<ServiceResultDTOWithTypeModel<StoreResponseModel>>
{
  public int StoreId { get; set; }
}
