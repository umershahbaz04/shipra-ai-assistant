using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Query.GetAllLowQuantityProductStock;
public class GetAllLowQuantityProductStockQuery : LowQuantityProductStockFilterModel,IRequest<ServiceResultDTO>
{ 
}
