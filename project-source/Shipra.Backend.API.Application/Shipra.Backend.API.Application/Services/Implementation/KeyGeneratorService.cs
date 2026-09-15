using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class KeyGeneratorService : IKeyGeneratorService
{
  private readonly IConfiguration Configuration;

  public KeyGeneratorService(IConfiguration configuration)
  {
    Configuration = configuration;
  }
  public string EncryptString(string plaintext)
  {
    if (!string.IsNullOrEmpty(plaintext))
    {
      var key = Configuration?.GetSection("AESKey")["Key"]?.ToString();

      var encypt = Utility.Encrypt(plaintext, key);
      return encypt;
    }
    return "";
  }
  public string DecryptString(string encryptedKey)
  {
    if (!string.IsNullOrEmpty(encryptedKey))
    {
      var key = Configuration?.GetSection("AESKey")["Key"]?.ToString();

      var decrypted = Utility.Decrypt(encryptedKey, key);
      return decrypted;
    }
    return "";
  } 
}
