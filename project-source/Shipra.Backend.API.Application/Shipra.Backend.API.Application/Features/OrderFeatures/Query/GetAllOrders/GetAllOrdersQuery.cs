using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrders;
public class GetAllOrdersQuery : CommonOrderFilters,IRequest<ServiceResultDTO>
{
   
}

