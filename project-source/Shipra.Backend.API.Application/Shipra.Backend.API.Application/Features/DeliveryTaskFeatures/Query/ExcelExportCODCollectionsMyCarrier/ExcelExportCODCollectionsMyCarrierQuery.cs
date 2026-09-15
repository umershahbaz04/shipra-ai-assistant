using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.ExcelExportCODCollectionsMyCarrier;
public class ExcelExportCODCollectionsMyCarrierQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? StoreId { get; set; }
  public string? CarrierIds { get; set; }
  public int? OrderTypeId { get; set; }
}
