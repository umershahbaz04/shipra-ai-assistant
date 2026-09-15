using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.NotificationAggregate;

namespace Shipra.Backend.API.Application.DTOs.NotificationUseCase;
public class CreateUpdateNotificationDto
{
  public string? NotificationConfigId { get; set; } 
  public string? Text { get; set; } 
  public int NotificationTypeId { get; set; }
  public int ServiceTypeId { get; set; }
  public int? NotificationEventId { get; set; }
  public string? Config { get; set; }
}
