using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface ISmsService
{
  Task SendSms(Message message, ClientId clientId,int? sMSActivateId = 0); 
}
