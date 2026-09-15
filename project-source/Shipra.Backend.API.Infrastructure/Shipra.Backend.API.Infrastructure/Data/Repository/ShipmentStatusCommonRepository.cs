using Dapper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class ShipmentStatusCommonRepository : IShipmentStatusCommonRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  //private readonly AppDbContext _context;

  public ShipmentStatusCommonRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    //_context = context;
  }
  public async Task<List<ShipmentDashboardResponseModel>> GetAllShipmentGridClientSettings(string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT sgc.ShipmentGridColumnId,
                     sgc.ColumnName AS DashboardStatusName,
                     REPLACE(sgc.ColumnName, ' ', '') AS DashboardStatusNameForKey,
                     sgc.DisplayOrder,
                     sgc.IsDisplay,
                     sgc.IsCompleted,
                     ISNULL(sgc.Active, 0) AS Active,
                     ISNULL(sgc.IsDefaultStatusTab, 1) AS IsDefaultStatusTab,
                     CASE
                         WHEN sgc.IsFetchAllPendingStatus = 1 THEN
                         (
                             SELECT STRING_AGG(ccts.CarrierTrackingStatusId, ',') AS CommaSeparatedIds
                             FROM dbo.ClientCarrierTrackingStatus AS ccts
                             WHERE ccts.ClientId = '{clientId}'
                                   AND NOT EXISTS
                             (
                                 SELECT 1
                                 FROM STRING_SPLIT(
                                      (
                                          SELECT STRING_AGG(sgcs.DashboardStatusValue, ',') AS AllDashboardStatusValues
                                          FROM dbo.ShipmentGridClientSetting AS sgcs
                                              INNER JOIN dbo.ShipmentGridColumn AS sgc
                                                  ON sgc.ShipmentGridColumnId = sgcs.ShipmentGridColumnId
                                                     AND sgcs.ClientId = '{clientId}'
                                          WHERE sgc.IsFetchAllPendingStatus IS NULL
                                                OR sgc.IsFetchAllPendingStatus <> 1
                                                   AND sgc.Active = 1
                                      ), ',')
                                 WHERE value = ccts.CarrierTrackingStatusId
                             )
                         )
                         ELSE
                             sgcs.DashboardStatusValue
                     END DashboardStatusValue
              FROM dbo.ShipmentGridColumn AS sgc
                  INNER JOIN dbo.ShipmentGridClientSetting AS sgcs
                      ON sgcs.ShipmentGridColumnId = sgc.ShipmentGridColumnId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (sgc.ClientId = @ClientId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY sgc.DisplayOrder";

      var data = await connection.QueryAsync<ShipmentDashboardResponseModel>(queryData, dynamicParams);
      return data.ToList();
    }
  }
}
