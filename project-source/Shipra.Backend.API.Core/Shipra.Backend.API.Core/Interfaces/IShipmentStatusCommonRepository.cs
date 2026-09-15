using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IShipmentStatusCommonRepository
{
  Task<List<ShipmentDashboardResponseModel>> GetAllShipmentGridClientSettings(string? clientId);
}
