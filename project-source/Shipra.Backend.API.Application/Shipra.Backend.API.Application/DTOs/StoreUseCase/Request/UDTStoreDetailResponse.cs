using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;

namespace Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;

public class UDTStoreDetailResponse
{
  public IList<CreateUploadStoreResponseModel>? UDTStoreDetail { get; set; }
  public bool IsSuccessed { get; set; }
  public IList<UDTFileUploadError>? Errors { get; set; }
}
public class CreateUploadStoreResponseModel
{
  public int RowNum { get; set; }
  public int? StoreId { get; set; }
  public string? StoreName { get; set; }
  public string? StoreCode { get; set; }
  public string? StoreCompany { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? LicenseNo { get; set; }
  public string? StoreImage { get; set; }
  public string? UserName { get; set; }
  public string? Password { get; set; }
  public AddressResponseDTO? StoreAddress { get; set; }
}
