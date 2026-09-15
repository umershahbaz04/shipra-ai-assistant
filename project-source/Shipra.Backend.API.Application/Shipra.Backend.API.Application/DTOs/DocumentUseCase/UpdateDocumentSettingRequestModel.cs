using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.DocumentUseCase;
public class UpdateDocumentSettingRequestModel
{ 
  public int DocumentTemplateId { get; set; }
  public string? TemplateName { get; set; }
  public string? DocumentTemplateConfigId { get; set; }
  public bool? IsAskEveryTime { get; set; }
}
