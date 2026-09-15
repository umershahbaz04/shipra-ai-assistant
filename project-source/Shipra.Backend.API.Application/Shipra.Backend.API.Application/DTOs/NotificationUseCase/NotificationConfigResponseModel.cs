namespace Shipra.Backend.API.Application.DTOs.NotificationUseCase;

public class NotificationConfigResponseModel
{
  public string? NotificationConfigId { get; set; }
  public string? Text { get; set; }
  public int? NotificationChannelId { get; set; } 
  public int? NotificationEventId { get; set; }
  public string? Config { get; set; } 
  public bool? Active { get; set; }

}
