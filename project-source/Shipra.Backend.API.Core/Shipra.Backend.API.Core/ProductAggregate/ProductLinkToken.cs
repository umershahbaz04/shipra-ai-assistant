using System.Security.Cryptography;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public partial class ProductLinkToken
{
  public ProductLinkTokenId? ProductLinkTokenId { get; set; }
  public ProductId? ProductId { get; set; }
  public int? StoreId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? Token { get; set; }
  public DateTime ExpiryOn { get; set; }
  public DateTime CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }

  public static ProductLinkToken Create(ProductId productId, int? storeId, ClientId clientId, string? token, EmployeeId employeeId)
  {
    return new ProductLinkToken()
    {
      ProductLinkTokenId = ProductLinkTokenId.New,
      ProductId = productId,
      StoreId = storeId,
      ClientId = clientId,
      Token = token,
      ExpiryOn = DateTime.UtcNow.AddDays(7),
      CreatedOn = DateTime.UtcNow,
      CreatedBy = employeeId
    };
  }

  public static string GenerateShortToken(int length = 6)
  {

    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    var randomBytes = new byte[length];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);

    var result = new char[length];
    for (int i = 0; i < length; i++)
    {
      result[i] = chars[randomBytes[i] % chars.Length];
    }

    return new string(result);
  }
}
public sealed record ProductLinkTokenId(Guid Value)
{
  public static ProductLinkTokenId New => new(Guid.NewGuid());
}
