namespace Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
public class ProductStationTransferRequestModel
{

  public string? TrackingNo { get; set; }

  public int? OriginProductStationId { get; set; }

  public int? DestinationProductStationId { get; set; }

  public DateTime? ExpectedArrivalTime { get; set; }

  public int? TransferStatusId { get; set; }
}
