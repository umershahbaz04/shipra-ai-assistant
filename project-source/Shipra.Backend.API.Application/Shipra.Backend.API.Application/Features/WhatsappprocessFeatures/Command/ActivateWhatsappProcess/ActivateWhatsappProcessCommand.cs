using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Command.ActivateWhatsappProcess;
public class ActivateWhatsappProcessCommand : IRequest<ServiceResultDTO>
{
  public Dictionary<string, string>? InputParameters { get; set; }
  public bool? IsDefault { get; set; }
  public bool? IsActive { get; set; }
}
