using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFile;
public class UploadProductFileCommand : IRequest<ServiceResultDTO>
{
  public IFormFile? File { get; set; }
}
