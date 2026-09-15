using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;

public class ImageGallery
{
  public long ImageGalleryId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? ImageUrl { get; set; }
  public int? MediaTypeId { get; set; }
  public string? FileName { get; set; }
  public string? Description { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public bool? Active { get; set; }

  public static ImageGallery Create(ClientId? clientId, string? imageUrl, int? mediaTypeId, string? fileName, string? description,EmployeeId employeeId)
  {
    return new ImageGallery
    {
      ClientId = clientId,
      ImageUrl = imageUrl,
      MediaTypeId = mediaTypeId,
      FileName = fileName,
      Description = description,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = employeeId,
      Active = true
    };
  }
}
