using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailByUplaodReturnReportFile;
public class GetOrderDetailByUplaodReturnReportFileQuery : IRequest<ServiceResultDTO>
{
  public IFormFile? File { get; set; }
  public int CarrierId { get; set; }
}
