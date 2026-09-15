using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.StoreUseCase.Request;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.UpdateStoreCommand;
public class UpdateStoreCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int StoreId { get; set; }
  public string? StoreName { get; set; } 
  public string? StoreCompany { get; set; } 
  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? Email { get; set; }
  public string? Urls { get; set; }
  public string? StoreImage { get; set; }
  public string? LicenseNo { get; set; }

  public UpdateStoreAddressRequestModel? Address { get; set; }
}
