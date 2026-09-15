using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.UpdateDeliveryTask;
public class UpdateDeliveryTaskCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryTaskId { get; set; }
  public string? JobCode { get; set; }
  public OrderId? OrderId { get; set; }
  public DriverId? DriverId { get; set; }
  public int? DriverPaid { get; set; }
  public DateTime? DriverPaidDate { get; set; }
  public int? DeliveryTaskStatusId { get; set; }
  public DriverReceivableId? DriverReceivableId { get; set; }
  public int? LastStatusUpdateId { get; set; }
  public bool? Active { get; set; }
  public int? SortOrder { get; set; }
}

