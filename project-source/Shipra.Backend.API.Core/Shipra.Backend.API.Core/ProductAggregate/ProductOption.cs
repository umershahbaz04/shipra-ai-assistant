using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductOption : EntityBase, IAggregateRoot
{
  public ProductOption()
  {

  }

  public ProductOptionsId? ProductOptionsId { get; private set; }
  public ProductId? ProductId { get; private set; }
  public int? OptionId { get; private set; }
  public string? OptionValue { get; private set; }
  public int? DisplayOrder { get; private set; }
  public bool? IsDeleted { get; private set; }


  public static ProductOption CreateProductOption(ProductId? productId, string? optionId, string? optionValue, int? displayOrder)
  {
    int? opId = int.TryParse(optionId, out int parsedValue) ? parsedValue : null;

    return new ProductOption()
    {
      ProductOptionsId = ProductOptionsId.New,
      ProductId = productId,
      OptionId = opId,
      OptionValue = optionValue,
      DisplayOrder = displayOrder,
      IsDeleted = false
    };
  }

  public ProductOption UpdateOption(ProductOptionsId? productOptionsId, ProductId productId, string? optionId, string? optionValue, int displayOrder, bool isDeleted)
  {
    int? opId = int.TryParse(optionId, out int parsedValue) ? parsedValue : null;

    ProductOptionsId = productOptionsId;
    ProductId = productId;
    OptionId = opId;
    OptionValue = optionValue;
    DisplayOrder = displayOrder;
    IsDeleted = isDeleted;

    return this;
  }
}
public sealed record ProductOptionsId(Guid Value)
{
  public static ProductOptionsId New => new(Guid.NewGuid());
}
