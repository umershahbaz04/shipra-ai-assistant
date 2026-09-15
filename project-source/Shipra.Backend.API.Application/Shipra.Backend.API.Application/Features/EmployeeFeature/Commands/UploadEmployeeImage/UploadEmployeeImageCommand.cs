using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UploadEmployeeImage;
public class UploadEmployeeImageCommand : IRequest<ServiceResultDTO>
{
  public IFormFile? File { get; set; }
}
