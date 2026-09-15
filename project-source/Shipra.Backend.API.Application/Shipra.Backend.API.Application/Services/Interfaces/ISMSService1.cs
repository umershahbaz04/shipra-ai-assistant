using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface ISMSService1
{
  Task SendSMSTrackingUpdate(long shipment_Id); 
}
