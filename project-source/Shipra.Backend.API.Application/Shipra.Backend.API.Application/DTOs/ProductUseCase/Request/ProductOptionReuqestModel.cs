using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

public class ProductOptionReuqestModel
{
  public string? ProductOptionsId { get; set; }
  public string? OptionId { get; set; }
  public string? OptionValue { get; set; }
  public int DisplayOrder { get; set; }
  public bool IsDeleted { get; set; }
}

public class ProductOptionUplaoadResponseModel
{
  public int? ProductOptionsId { get; set; } 
  public string? OptionValue { get; set; }
  public int DisplayOrder { get; set; } 
}
public class ProductMediaRequestModel
{
  public long ProductMediaId { get; set; }
  public long MediaTypeId { get; set; }
  public long ImageGalleryId { get; set; }  
  public bool? IsFeatured { get; set; } 
}
