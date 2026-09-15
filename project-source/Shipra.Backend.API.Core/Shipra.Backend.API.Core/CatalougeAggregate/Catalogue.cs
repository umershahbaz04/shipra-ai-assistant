using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.CatalougeAggregate;
public class Catalogue
{
  public int CatalogueId { get; private set; }
  public string? ClientId { get; private set; }
  public int? DatabaseId { get; private set; }
  public bool? Active { get; set; }
  public int? OperationalStatusId { get; set; }
  public string? ClientIdentifier { get; set; }
  public static Catalogue Create(int databaseId, string clientId)
  {
    return new Catalogue
    {
      DatabaseId = databaseId,
      ClientId = clientId,
      Active = true,
    };
  }
  public void UpdateClientIdentifier(string? clientIdentifier)
  {
    ClientIdentifier = clientIdentifier; 
  }
}
