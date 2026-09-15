using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.ExcelExportAllOrders;
public class ExcelExportAllOrdersQuery : CommonOrderFilters,IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
   
}
