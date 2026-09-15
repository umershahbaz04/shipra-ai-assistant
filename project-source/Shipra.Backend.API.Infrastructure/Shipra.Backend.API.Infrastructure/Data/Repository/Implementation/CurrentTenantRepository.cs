using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class CurrentTenantRepository : ICurrentTenantRepository
{
  private readonly IDbContextService _dbContextService;

  public CurrentTenantRepository(IDbContextService dbContextService)
  {
    _dbContextService = dbContextService;
  }
  public string GetConnectionStringByTenant(string clientId)
  {
    return _dbContextService.GetConnectionString(clientId);
  }

  public OperationStatusResponseModel GetConnectionStringByTenantWithOperation(string clientId)
  {
    return _dbContextService.GetConnectionWithOperationaStatus(clientId); 
  }
}
