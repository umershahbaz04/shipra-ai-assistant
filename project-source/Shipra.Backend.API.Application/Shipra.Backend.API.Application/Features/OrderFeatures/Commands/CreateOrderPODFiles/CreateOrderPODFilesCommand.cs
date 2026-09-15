using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderPODFiles;
public class CreateOrderPODFilesCommand : IRequest<ServiceResultDTO>
{
  public List<IFormFile>? FilesList { get; set; }
  public string? OrderId { get; set; }
  public string? Comment { get; set; }
}
