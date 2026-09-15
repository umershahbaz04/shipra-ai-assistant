using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Response;
public class ImageGalleryResponseDto
{ 
    public long ImageGalleryId { get; set; } 
    public string? ImageUrl { get; set; }
    public int? MediaTypeId { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedOn { get; set; }
    public bool? Active { get; set; }  
}
