using Grpc.Core;
using Shipra.Backend.API.Core.Grpc;
using Shipra.GrpcContract.Protos;
using Shipra.GrpcContracts;

namespace Shipra.Backend.API.Infrastructure.Grpc;

public class GrpcClientService : IGrpcClientService
{
  //public Task<string> SendNotificationAsync(string message)
  //{
  //  throw new NotImplementedException();
  //}

  private readonly NotificationService.NotificationServiceClient _client;
  private readonly WebhookService.WebhookServiceClient _webhookServiceClient;
  private readonly FollowupService.FollowupServiceClient _followupServiceClient;
  private readonly GeneralService.GeneralServiceClient _generalServiceClient;

  public GrpcClientService(GrpcClientFactory grpcClientFactory)
  {
    _client = grpcClientFactory.GetClient();
    _webhookServiceClient = grpcClientFactory.GetWebhookServiceClient();
    _followupServiceClient = grpcClientFactory.GetFollowupServiceClient();
    _generalServiceClient = grpcClientFactory.GetGeneralServiceClient();
  }

  public async Task<string> SendMessageAsync(string clientId, string message)
  {
    var headers = new Metadata
        {
            { "X-TenantId", clientId }
        };

    var request = new NotificationRequest { Message = message };
    var reply = await _client.SendNotificationAsync(request, headers);


    return reply.Confirmation;
  } 
  public async Task<string> SendWebhookMessageAsync(string clientId, string message)
  {
    var headers = new Metadata
        {
            { "X-TenantId", clientId }
        };

    var request = new WebhookRequest { Message = message };
    var reply = await _webhookServiceClient.SendWebhookAsync(request, headers);


    return reply.Confirmation;
  }  
  public async Task<string> SendFollowupMessageAsync(string clientId, string message)
  {
    var headers = new Metadata
        {
            { "X-TenantId", clientId }
        };

    var request = new FollowupRequest { Message = message };
    var reply = await _followupServiceClient.SendFollowupAsync(request, headers);


    return reply.Confirmation;
  }
  public async Task<string> SendGeneralMessageAsync(string clientId, string message)
  {
    var headers = new Metadata
        {
            { "X-TenantId", clientId }
        };

    var request = new GeneralRequest { Message = message };
    var reply = await _generalServiceClient.SendGeneralAsync(request, headers);


    return reply.Confirmation;
  }
}
