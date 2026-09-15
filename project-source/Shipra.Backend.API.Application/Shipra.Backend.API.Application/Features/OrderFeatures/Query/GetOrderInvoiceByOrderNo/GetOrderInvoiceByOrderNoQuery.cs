using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
public class GetOrderInvoiceByOrderNoQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}

