using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
public class CreateImageGalleryRequestDto
{
  public IFormFile? File { get; set; }
  public string? Description { get; set; }
}
