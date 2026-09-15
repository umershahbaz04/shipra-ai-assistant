using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Security.Service.IManagers;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetSecretKeyDecryptedUserNamePassword;
public class GetSecretKeyDecryptedUserNamePasswordQuery : IRequest<ServiceResultDTO>
{
  public string? EncryptedKey { get; set; }
}
public class GetSecretKeyDecryptedUserNamePasswordQueryValidator : RequestHandlerBase<GetSecretKeyDecryptedUserNamePasswordQuery, ServiceResultDTO>
{
  private readonly IKeyGeneratorManager _keyGeneratorManager;

  public GetSecretKeyDecryptedUserNamePasswordQueryValidator(IKeyGeneratorManager keyGeneratorManager, IServiceProvider serviceProvider, ILogger<GetSecretKeyDecryptedUserNamePasswordQueryValidator> logger) : base(serviceProvider, logger)
  {
    _keyGeneratorManager = keyGeneratorManager;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSecretKeyDecryptedUserNamePasswordQuery request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      await Task.Delay(1);
      string? decryptedValue = _keyGeneratorManager.DecryptString(request.EncryptedKey!);
      if (!string.IsNullOrEmpty(decryptedValue))
      {
        var desc = JsonConvert.DeserializeObject<KeyModel>(decryptedValue!);
        if (desc != null)
        {
          response = new ServiceResultDTO(desc!);
        }
        else
        {
          response.CreateError("Error", new string[] { "Data not found against key:" + request.EncryptedKey });
        }
      }

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }

  }
}
