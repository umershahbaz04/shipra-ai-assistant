using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPActivateByPPActivateId;
public class GetPPActivateByPPActivateIdQuery : IRequest<ServiceResultDTO>
{
  public int PPactivateId { get; set; }
}
