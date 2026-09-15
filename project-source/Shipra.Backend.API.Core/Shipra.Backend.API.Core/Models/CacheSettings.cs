namespace Shipra.Backend.API.Core.Models;
public class CacheSettings
{
  public int AbsoluteExpirationInMinutes { get; set; }
  public int SlidingExpirationInMinutes { get; set; }
  public bool ClearCacheOnRequest { get; set; }
}
