using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface ISaleChannelOrderService
{
  Task SaleChannelsOrderManageQueuesAsync();
}
