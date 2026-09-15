using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllDeliveryTask;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportAllDeliveryTasks;

public class ExcelExportAllDeliveryTasksQuery : DeliveryTaskFilter, IRequest<ServiceResultDTOWithTypeModel<ExcelResponseModel>>
{
}
