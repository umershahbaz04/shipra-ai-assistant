using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.RootFinding;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface ICurrentTenantService
{
  string GetConnectionStringByTenant(string clientId);
  string GetTenantAsync(HttpContext context);
  OperationStatusResponseModel GetTenantAsyncWithOperation(HttpContext context);
}
