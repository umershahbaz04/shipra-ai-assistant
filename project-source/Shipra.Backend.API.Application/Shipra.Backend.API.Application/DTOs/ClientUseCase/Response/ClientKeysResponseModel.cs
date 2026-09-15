namespace Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;

public class ClientKeysResponseModel
{
  public string? PublicKey { get; set; }
  public string? SecretKey { get; set; } 
  public string? EncryptedKey { get; set; }
}
