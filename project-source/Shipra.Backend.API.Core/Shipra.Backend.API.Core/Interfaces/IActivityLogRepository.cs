using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ActivityLogAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IActivityLogRepository
{
  Task<bool> Create(ActivityLog activityLog);
}
