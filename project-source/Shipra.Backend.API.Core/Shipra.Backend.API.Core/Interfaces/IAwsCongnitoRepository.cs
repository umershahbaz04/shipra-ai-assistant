using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IAwsCongnitoRepository
{
  Task<List<UserPoolClientResponseModel>?> GetAllUserPoolClients(bool? clearCacheOnRequest = false);
  Task<UserPoolClientResponseModel?> GetUserPoolClientByClientId(string clientId);
}
