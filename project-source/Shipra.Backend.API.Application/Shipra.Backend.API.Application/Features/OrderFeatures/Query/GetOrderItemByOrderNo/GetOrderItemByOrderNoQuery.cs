using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderItemByOrderNo;
public class GetOrderItemByOrderNoQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
