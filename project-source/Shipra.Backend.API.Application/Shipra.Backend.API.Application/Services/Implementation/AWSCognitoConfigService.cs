using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class AWSCognitoConfigService : IAWSCognitoConfigService
{
  private readonly ExceptionHandlerService _exceptionHandlerService;
  private readonly IAwsCongnitoRepository _awsCongnitoRepository;

  public AWSCognitoConfigService(ExceptionHandlerService exceptionHandlerService,IAwsCongnitoRepository awsCongnitoRepository)
  {
    _exceptionHandlerService = exceptionHandlerService;
    _awsCongnitoRepository = awsCongnitoRepository;
  }
  //List<DataModel> dataList = new List<DataModel>
  //      {
  //          new DataModel { TenantClientId="C103DD6A-A081-70C6-DBC4-A09CA2B08FDC", ClientName = "afaq12",    UserPoolId = "ap-south-1_eI8CqYgUF",UserPoolClientId="ko6t6slat0ouus658snlmmk6p", Region = "ap-south-1", },
  //          new DataModel { TenantClientId="91D3DDAA-10F1-7020-A35E-E1487FC73F85", ClientName = "ali01", UserPoolId = "ap-south-1_HdZt8ou0r",UserPoolClientId="1aba6ntqjjjtrs2ch0locvmp0j", Region = "ap-south-1"  }
  //};
  public async Task<AWSOptions> GetAWSUserClientOptionsAsync(string clientId,string userPoolId,string username)
  {
    // Retrieve the UserPoolId and Region from the database based on the clientId
    try
    {
      var alllClients = await _awsCongnitoRepository.GetAllUserPoolClients();
      //get client by username and pool id
      var config = alllClients?.Where(x => x.UserPoolId == userPoolId && x.ClientName == username).FirstOrDefault();
      if (config is null)
      {
        alllClients = await _awsCongnitoRepository.GetAllUserPoolClients(true);
        config = alllClients?.Where(x => x.UserPoolId == userPoolId && x.ClientName == username).FirstOrDefault();
      }
      //if (userPoolClients != null)
      //{
      //  if (oUserPoolClientTarget != null)
      //  { 
      //  }
      //}

      //var config = alllClients!.FirstOrDefault(c => c.TenantClientId != null
      //                          && string.Equals(c.TenantClientId.Value.ToString(), clientId, StringComparison.OrdinalIgnoreCase));
      if (config != null)
      {
        return new AWSOptions
        {
          UserPoolId = config.UserPoolId,
          Region = config.Region,
          UserPoolClientId = config.PoolClientId
        };
      }
    }
    catch (Exception ex)
    {
      _ = ex.Message;
      var reqBody =  "AwsConfigurationservice";
      string? clientInfo = _exceptionHandlerService.GetClientIdFromRequest(clientId, username);
      _exceptionHandlerService.SendEmail(clientInfo, reqBody, ex);
    }

    return new AWSOptions();
  }
}
public class DataModel
{
  public string? TenantClientId { get; set; }
  public string? ClientName { get; set; }
  public string? Region { get; set; }
  public string? UserPoolId { get; set; }
  public string? UserPoolClientId { get; set; }
}

