namespace Shipra.Backend.API.Core.Grpc;
public interface IGrpcClientService
{
  Task<string> SendMessageAsync(string clientId, string message);
  Task<string> SendWebhookMessageAsync(string clientId, string message);
  Task<string> SendFollowupMessageAsync(string clientId, string message);
  Task<string> SendGeneralMessageAsync(string clientId, string message);
  // Add other methods if needed
}
