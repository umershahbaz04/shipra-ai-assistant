using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Commands.CreateProductStationTransfer;
public class CreateProductStationTransferCommand : CommandBase, IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{

  public string? TrackingNo { get; set; }

  public int? OriginProductStationId { get; set; }

  public int? DestinationProductStationId { get; set; }

  public DateTime? ExpectedArrivalTime { get; set; }

  public int? TransferStatusId { get; set; }
  public Guid? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public List<TransferProductRequestModel>? TransferProducts { get; set; } = new();
}
