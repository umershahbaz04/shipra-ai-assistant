using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteDriverReceivable;
public class DeleteDriverReceivableCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? DriverReceivableId { get; set; }
}
