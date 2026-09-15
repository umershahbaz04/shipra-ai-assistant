using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Command.UploadShipraStaticContent;
public class UploadShipraStaticContentCommand : IRequest<ServiceResultDTOWithTypeModel<S3ResponseDTO>>
{
  public IFormFile? File { get; set; }
  public int? SCFolderLookupId { get; set; }
  public string? Path { get; set; }
}
