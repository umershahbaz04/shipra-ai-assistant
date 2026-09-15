using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class WhatsAppCategoryType : EntityBase, IAggregateRoot
{
  public WhatsAppCategoryType() { }
  public int WhatsAppCategoryTypeId { get; private set; }
  public string? Name { get; private set; }
}

