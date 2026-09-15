using MediatR;
using Shipra.Backend.API.Application.DTOs;
using System.Collections.Generic;

namespace Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;

public class UpdateMetaFieldForOrdersCommand : IRequest<ServiceResultDTO>
{
  public List<string>? OrderIds { get; set; }
  public string? SettingConfig { get; set; }
}
