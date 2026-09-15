using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductFile : EntityBase, IAggregateRoot
{
  public ProductFile(ProductFileId? productFileId, ProductId? productId, string? path, string? type, string? createdBy, DateTime? createdOn, string? updatedBy, DateTime? updatedOn)
  {
    ProductFileId = productFileId;
    ProductId = productId;
    Path = path;
    Type = type;
    CreatedBy = createdBy;
    CreatedOn = createdOn;
    UpdatedBy = updatedBy;
    UpdatedOn = updatedOn;
  }

  public ProductFileId? ProductFileId { get; set; } 
  public ProductId? ProductId { get; set; } 
  public string? Path { get; set; } 
  public string? Type { get; set; } 
  public string? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public string? UpdatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; }
} 
public sealed record ProductFileId(Guid Value)
{
  public static ProductFileId New => new(Guid.NewGuid());
}
