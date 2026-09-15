using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;

namespace Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Response;
public class ProductStationTransferResponseModel
{
  public string? ProductStaionTransferId { get; set; }

  public string? TransferNo { get; set; }

  public string? TrackingNo { get; set; }

  public int? OriginProductStationId { get; set; }

  public int? DestinationProductStationId { get; set; }

  public DateTime? ExpectedArrivalTime { get; set; }

  public int? TransferStatusId { get; set; }
  public List<TransferProductRequestModel> TransferProducts { get; set; } = new();
}
