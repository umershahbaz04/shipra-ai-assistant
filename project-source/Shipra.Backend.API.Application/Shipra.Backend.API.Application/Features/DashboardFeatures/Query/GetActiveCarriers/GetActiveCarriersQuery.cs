using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.DashboardUserCase;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetActiveCarriers;
public class GetActiveCarriersQuery : IRequest<ServiceResultDTOWithTypeModel<DashboardDataWithCountResponseModel>>
{
}
