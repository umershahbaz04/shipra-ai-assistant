using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ShopifyAggregate;
public class ShopifyConfig
{
  public int ShopifyConfigId { get; private set; }
  public int SaleChannelConfigId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public string? AccessToken { get; private set; }
  public string? RefreshToken { get; private set; }
  public string? ShopDomain { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static ShopifyConfig CreateShopifyConfig(string accessToken, string shopDomain, int saleChannelConfigId, ClientId clientId, EmployeeId createdBy)
  {
    var shopifyConfig = new ShopifyConfig()
    {
      AccessToken = accessToken,
      SaleChannelConfigId = saleChannelConfigId,
      ShopDomain = shopDomain,
      RefreshToken = null,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
    return shopifyConfig;
  }

  public void UpdateShopifyConfig(string accessToken, EmployeeId updatedBy)
  {
    AccessToken = accessToken;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
    Active = true;
  }

  public void UpdateShopifyConfigRefreshToken(string refreshToken, EmployeeId updatedBy)
  {
    RefreshToken = refreshToken;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
    Active = true;
  }

  public void DeleteShopifyConfig(EmployeeId? userId)
  {
    AccessToken = null;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
    Active = false;
  }
}
