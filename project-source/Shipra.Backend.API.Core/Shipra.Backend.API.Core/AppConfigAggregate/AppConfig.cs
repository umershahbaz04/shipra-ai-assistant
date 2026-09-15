namespace Shipra.Backend.API.Core.AppConfigAggregate;
public class AppConfig
{
  public int AppConfigId { get; set; }
  public string? AppConfigKey { get; set; }
  public string? AppConfigValue { get; set; }
  public bool? Active { get; set; }

  public static AppConfig CreateAppConfig(string appConfigKey, string appConfigValue)
  {
    return new AppConfig()
    {
      AppConfigKey = appConfigKey,
      AppConfigValue = appConfigValue,
      Active = true
    };
  }
  public void DeleteAppConfig()
  {
    Active = false;
  }

  public void UpdateAppConfig(string appConfigKey, string appConfigValue)
  {
    AppConfigKey = appConfigKey;
    AppConfigValue = appConfigValue;
  }
}

