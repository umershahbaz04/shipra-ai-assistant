using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UploadClientImage;
public class UploadClientImageCommand : IRequest<ServiceResultDTOWithTypeModel<S3ResponseDTO>>
{
  public IFormFile? File { get; set; }
}
