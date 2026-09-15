using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ZokoWhatsappModel
{
  public string channel { get; set; } = "whatsapp";
  public string[]? templateArgs { get; set; }
  public string? recipient { get; set; }
  public string? type { get; set; }
  public string? message { get; set; }
  public string? templateId { get; set; }
  public string? templateLanguage { get; set; }
}

public class TemplateArgs
{
  public string? recipient { get; set; }
  public string? templateId { get; set; }
  public string? type { get; set; }
  public string? templateLanguage { get; set; }
  public string[]? templateArgs { get; set; }
  public string? message { get; set; }
}
