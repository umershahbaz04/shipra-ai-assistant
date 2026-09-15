using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.NotificationAggregate;
using Shipra.Backend.API.Core.PaymentProcessAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class NotificationRepository : INotificationRepository
{
  private readonly AppDbContext _context;
  //dont use dapper here
  public NotificationRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<bool> CreaetNotificationConfig(NotificationConfig notificationConfig)
  {
    await _context.NotificationConfigs.AddAsync(notificationConfig);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateNotificationConfig(NotificationConfig notificationConfig)
  {
    _context.NotificationConfigs.Update(notificationConfig);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> DeleteNotificationConfig(NotificationConfig notificationConfig)
  {
    _context.NotificationConfigs.Remove(notificationConfig);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetNotificationConfigByTypeId(int notificationTypeId, ClientId? clientId)
  {
    var result = await _context.NotificationConfigs
        .Where(nc => nc.ClientId == clientId)
        .Join(_context.NotificationChannels,
              nc => nc.NotificationChannelId,
              nc2 => nc2.NotificationChannelId,
              (nc, nc2) => new { NotificationConfig = nc, NotificationChannel = nc2 }).Where(x => x.NotificationChannel.NotificationTypeId == notificationTypeId)
               .Select(x => new
               {
                 // NotificationConfig properties
                 NotificationConfigId = x.NotificationConfig!.NotificationConfigId!.Value!.ToString(),
                 Text = x.NotificationConfig.Text,
                 NotificationChannelId = x.NotificationConfig.NotificationChannelId,
                 NotificationEventId = x.NotificationConfig.NotificationEventId,
                 Config = x.NotificationConfig.Config,
                 Active = x.NotificationConfig.Active,

                 // NotificationChannel properties
                 NotificationTypeId = x.NotificationChannel.NotificationTypeId,
                 ServiceTypeId = x.NotificationChannel.ServiceTypeId,
               })
              .ToListAsync();

    //var result = await _context.NotificationConfigs
    //    .Where(nc => nc.ClientId == clientId).Select(x => new
    //    {
    //      // NotificationConfig properties
    //      NotificationConfigId = x.NotificationConfigId!.Value!.ToString(),
    //      Text = x.Text,
    //      NotificationChannelId = x.NotificationChannelId,
    //      NotificationEventId = x.NotificationEventId,
    //      Config = x.Config,
    //      Active = x.Active,

    //      // NotificationChannel properties
    //      NotificationTypeId = x.NotificationTypeId,
    //    }).ToListAsync();


    return result;
  }

  public async Task<List<NotificationEvent>> GetNotificationEvents()
  {
    return await _context.NotificationEvents.ToListAsync();
  }

  public async Task<List<NotificationType>> GetNotificationTypes()
  {
    return await _context.NotificationTypes.ToListAsync();
  }

  public async Task<NotificationConfig> GetNotificationConfigById(NotificationConfigId notificationConfigId, ClientId? clientId)
  {
    var target = await _context.NotificationConfigs.FirstOrDefaultAsync(x => x.NotificationConfigId == notificationConfigId && x.ClientId == clientId);
    return target!;
  }
  public async Task<List<NotificationConfig>> GetNotificationConfigByClientId(ClientId? clientId)
  {
    var target = await _context.NotificationConfigs.Where(x => x.ClientId == clientId).ToListAsync();
    return target!;
  }

  public async Task<bool> CreateNotificationChannel(NotificationChannel notificationChannel)
  {
    await _context.NotificationChannels.AddAsync(notificationChannel);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateNotificationChannel(NotificationChannel notificationChannel)
  {
    _context.NotificationChannels.Update(notificationChannel);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<NotificationChannel> GetNotificationChannelBySCId(int notificationChannelId)
  {
    var target = await _context.NotificationChannels.FirstOrDefaultAsync(x => x.NotificationChannelId == notificationChannelId);
    return target!;
  }
  public async Task<NotificationEvent?> GetNotificationEventsByName(string? eventName)
  {
    var target = await _context.NotificationEvents.FirstOrDefaultAsync(x => x.EventKey!.Trim().ToLower() == eventName!.Trim().ToLower());

    return target!;
  }
  public async Task<bool> DeleteNotificationChannel(NotificationChannel notificationChannel)
  {
    _context.NotificationChannels.Remove(notificationChannel);
    return await _context.SaveChangesAsync() > 0;
  }
  #region WhatsApp Button Handler
  public async Task<NotificationConfig> GetSingleNotificationConfigByClientId(ClientId? clientId)
  {
    var target = await _context.NotificationConfigs.FirstOrDefaultAsync(x => x.ClientId == clientId);

    return target!;
  }
  #endregion
}
