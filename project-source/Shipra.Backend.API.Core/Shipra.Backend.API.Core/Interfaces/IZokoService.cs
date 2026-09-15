using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IZokoService
{
  Task<WhatsAppResponse> SendLocationNotificationAsync(Order oOrder, OrderAddress orderAddress, ClientId clientId);
}
