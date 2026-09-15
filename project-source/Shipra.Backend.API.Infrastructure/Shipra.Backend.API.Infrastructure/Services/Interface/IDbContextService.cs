using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Html2pdf.Css.Resolve;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure.Data;

namespace Shipra.Backend.API.Infrastructure.Services.Interface;
public interface IDbContextService
{
  IDbConnection GetDapperDbConection(string clientId);
  AppDbContext GetAppDbContext(string clientId); 
  string GetConnectionString(string clientId);
  OperationStatusResponseModel GetConnectionWithOperationaStatus(string clientId);
  AppDbContext GetAppDbContextWithConnectionString(string connectionString);
}
