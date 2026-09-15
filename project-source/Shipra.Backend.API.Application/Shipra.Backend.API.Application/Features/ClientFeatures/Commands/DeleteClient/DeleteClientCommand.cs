using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.DeleteClient;
public class DeleteClientCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? ClientId { get; set; }
}
