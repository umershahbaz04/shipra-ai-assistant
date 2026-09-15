using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class ProductOptionLookup : EntityBase, IAggregateRoot
{
  public ProductOptionLookup() { }
  public int ProductOptionId { get; set; }
  public string? Name { get; set; }

  public static ProductOptionLookup AddDefault()
  {
    return new ProductOptionLookup()
    {
      ProductOptionId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
