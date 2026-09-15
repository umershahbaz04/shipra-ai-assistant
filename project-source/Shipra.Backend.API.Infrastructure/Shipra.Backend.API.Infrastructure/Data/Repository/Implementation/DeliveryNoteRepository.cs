using System.Dynamic;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DeliveryNoteRepository : IDeliveryNoteRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public DeliveryNoteRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  #region DeliveryNote
  public async Task<DeliveryNote> CreateDeliveryNote(DeliveryNote deliveryNote)
  {
    await _context.DeliveryNotes.AddAsync(deliveryNote);
    await _context.SaveChangesAsync();
    return deliveryNote;
  }
  public async Task<DeliveryNote> UpdateDeliveryNote(DeliveryNote deliveryNote)
  {
    _context.DeliveryNotes.Update(deliveryNote);
    await _context.SaveChangesAsync();
    return deliveryNote;
  }
  public async Task<DeliveryNote?> GetDriverActiveDeliveryNoteToday(DriverId driverId, ClientId clientId)
  {
    var regionMinutes = await CommonUtility.GetClientRegionMinutes(clientId.Value.ToString(), _context);
    var localNow = DateTime.UtcNow.AddMinutes(regionMinutes);
    var localToday = localNow.Date;

    var notes = await _context.DeliveryNotes
      .Where(x => x.DriverId == driverId && x.ClientId == clientId && x.DeliveryNoteStatusId == (int)EnumDeliveryNoteStatusLookup.InProgress && x.Active == true)
      .ToListAsync();

    return notes.FirstOrDefault(x => x.CreatedOn.HasValue && x.CreatedOn.Value.AddMinutes(regionMinutes).Date == localToday);
  }

  public async Task<dynamic> GetAllDeliveryNote(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, int deliveryNoteStatusId, string? clientId, string? driverIds = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string TotalCount = "Select COUNT(dn.DeliveryNoteId) ";
      TotalCount += @"FROM dbo.DeliveryNote AS dn
                        LEFT JOIN Driver AS d ON d.DriverId = dn.DriverId 
						            LEFT JOIN Employee as e ON e.EmployeeId=d.EmployeeId
                        LEFT JOIN dbo.DeliveryNoteStatusLookup as dnsl On dnsl.DeliveryNoteStatusId = dn.DeliveryNoteStatusId ";

      string query = @" SELECT dn.DeliveryNoteId,
                         dn.NoteNo,
                         dn.DriverId,
                         e.EmployeeName AS DriverName,
                         e.MobileNo AS Phone,
                         dn.ShipmentCount AS ShipmentCount,
                         ISNULL(dn.IsCompleted, 0) AS IsCompleted,
                         dnsl.DeliveryNoteStatusId,
                         dnsl.DeliveryNoteStatusName AS Status,
                         (
                             SELECT COUNT(*)
                             FROM dbo.DeliveryNoteDetail AS dnd
                             WHERE dnd.DeliveryNoteId = dn.DeliveryNoteId AND dnd.DeliveryNoteDetailStatusId = 1
                         ) AS TotalPendingCount,
                         dn.CreatedOn AS CreatedDate
                  FROM dbo.DeliveryNote AS dn
                      LEFT JOIN dbo.Driver AS d
                          ON d.DriverId = dn.DriverId
                      LEFT JOIN dbo.Employee AS e
                          ON e.EmployeeId = d.EmployeeId
                      LEFT JOIN dbo.DeliveryNoteStatusLookup AS dnsl
                          ON dnsl.DeliveryNoteStatusId = dn.DeliveryNoteStatusId  ";
      string whereStart = "WHERE ( dn.Active=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ((dn.NoteNo in (select value from STRING_SPLIT(@search,',')))) ";
      }
      if (!string.IsNullOrEmpty(driverIds))
      {
        dynamicParams.Add("@driverIds", driverIds);
        whereStart += "And (dn.DriverId in (select value from STRING_SPLIT(@driverIds,','))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (d.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dn.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dn.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      #region deliveryNoteStatusId
      if (deliveryNoteStatusId > 0)
      {
        dynamicParams.Add("@deliveryNoteStatusId", deliveryNoteStatusId);
        whereStart += "And (dn.DeliveryNoteStatusId = @deliveryNoteStatusId) ";
      }
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "dn.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      result.list = data.ToList();
      return result;
    }
  }

  public async Task<DeliveryNote?> GetDeliveryNoteById(DeliveryNoteId deliveryNoteId)
  {
    return await _context.DeliveryNotes.FirstOrDefaultAsync(x => x.DeliveryNoteId == deliveryNoteId && x.Active == true);
  }
  public async Task<DeliveryNote?> GetDeliveryNoteByNoteNo(string? noteNo, string? clientId)
  {
    if (string.IsNullOrEmpty(noteNo)) return null;

    if (!string.IsNullOrEmpty(clientId) && Guid.TryParse(clientId, out var clientGuid))
    {
      var clId = new ClientId(clientGuid);
      return await _context.DeliveryNotes
        .FirstOrDefaultAsync(dn => dn.NoteNo == noteNo && dn.ClientId == clId && dn.Active == true);
    }

    return await _context.DeliveryNotes
      .FirstOrDefaultAsync(dn => dn.NoteNo == noteNo && dn.Active == true);
  }
  public Task<string> GetDeliveryNoteNo(ClientId clientId)
  {
    throw new NotImplementedException();
  }
  public async Task<bool> DeleteDeliveryNoteById(DeliveryNote deliveryNote)
  {
    _context.DeliveryNotes.Remove(deliveryNote);
    await _context.SaveChangesAsync();
    return true;
  }
  #endregion

  #region DeliveryNoteDetail
  public async Task<DeliveryNoteDetail> CreateDeliveryNoteDetail(DeliveryNoteDetail deliveryNoteDetail)
  {
    await _context.DeliveryNoteDetails.AddAsync(deliveryNoteDetail);
    await _context.SaveChangesAsync();
    return deliveryNoteDetail;
  }
  public async Task<DeliveryNoteDetail?> GetDeliveryNoteDetailByOrderId(OrderId orderId)
  {
    return await _context.DeliveryNoteDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
  }

  public async Task<DeliveryNoteDetail?> GetDeliveryNoteDetailById(DeliveryNoteDetailId? deliveryNoteDetailId)
  {
    var target = await _context.DeliveryNoteDetails.FirstOrDefaultAsync(x => x.DeliveryNoteDetailId == deliveryNoteDetailId && x.Active == true);
    return target!;
  }

  public async Task<IEnumerable<dynamic>> GetDriverLatestDeliveryNoteToday(string driverId, string clientId)
  {
    var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
    string query = $@"SELECT dn.NoteNo, 
                             dn.CreatedOn,
                             (
                               SELECT COUNT(*)
                               FROM dbo.DeliveryNoteDetail dnd2
                               WHERE dnd2.DeliveryNoteId = dn.DeliveryNoteId 
                               AND dnd2.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Pending}
                             ) AS TotalPendingCount,
                             o.OrderNo, 
                             oa.CustomerName,
                             oa.CustomerFullAddress as DeliveryAddress,
                             o.OrderDate
                      FROM dbo.DeliveryNote dn
                      INNER JOIN dbo.Driver d ON d.DriverId = dn.DriverId
                      LEFT JOIN dbo.DeliveryNoteDetail dnd ON dnd.DeliveryNoteId = dn.DeliveryNoteId AND dnd.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Pending}
                      LEFT JOIN dbo.[Order] o ON o.OrderId = dnd.OrderId
                      LEFT JOIN dbo.OrderAddress AS oa ON oa.OrderAddressId = o.OrderAddressId
                      WHERE CAST(d.DriverId AS VARCHAR(50)) = '{driverId}'
                      AND CAST(dn.DeliveryNoteStatusId AS INT) != {(int)EnumDeliveryNoteStatusLookup.Completed}
                      AND CAST({CommonUtility.GetFormatedDateStr("dn.CreatedOn", regionMinuts)} AS DATE) = CAST({CommonUtility.GetFormatedDateStr("GETUTCDATE()", regionMinuts)} AS DATE)
                      ORDER BY dn.CreatedOn DESC";

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var result = await connection.QueryAsync<dynamic>(query);
      return result;
    }
  }

  public async Task<List<DeliveryNoteDetail>>? GetAllDeliveryNoteDetailByNoteId(DeliveryNoteId? deliveryNoteId)
  {
    var target = await _context.DeliveryNoteDetails.Where(x => x.DeliveryNoteId == deliveryNoteId && x.Active == true).ToListAsync();
    return target!;
  }

  public async Task<List<DeliveryNoteDetail>?> GetAllDeliveryNoteDetailByDeliveryNoteId(DeliveryNoteId? deliveryNoteId)
  {
    var target = await _context.DeliveryNoteDetails.Where(x => x.DeliveryNoteId == deliveryNoteId && x.Active == true).ToListAsync();
    return target!;
  }

  public async Task<dynamic> GetDeliveryNoteDetailForDebrief(string deliveryNoteId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY dn.CreatedOn)) AS RowNum,
                                           COUNT(*) OVER () AS TotalCount,
                                           dnd.DeliveryNoteDetailId,
                                           dn.DeliveryNoteId,
                                           dn.DeliveryNoteStatusId,
                                           O.OrderNo,
                                           O.OrderId,
                                           ISNULL(O.CarrierTrackingNo, '') AS TrackingNo,
                                           cts.TrackingStatus AS CarrierTrackingStatus,
                                           cts.CarrierTrackingStatusId,
                                           dndsl.DeliveryNoteDetailStatusName AS DeliveryNoteDetailStatus,
                                           dndsl.DeliveryNoteDetailStatusId,
                                           ISNULL(c.TrackingStatus, '') AS DriverLastUpdatedStatus,
                                           dnd.DriverLastUpdatedStatusId,
                                           PML.Code AS PaymentMethodStatus,
                                           O.PaymentMethodId,
                                           O.Amount,
                                           oa.CustomerName AS Customer,
                                           oa.Mobile1 AS Phone,
                                           oa.Mobile2 AS Phone2,
                                           O.Description,
                                           O.Remarks,
                                           ISNULL(O.OrderLabels, '') AS OrderLabels
                                    FROM dbo.DeliveryNoteDetail AS dnd
                                        INNER JOIN dbo.DeliveryNote AS dn
                                            ON dn.DeliveryNoteId = dnd.DeliveryNoteId
                                        INNER JOIN dbo.DeliveryNoteDetailStatusLookup AS dndsl
                                            ON dndsl.DeliveryNoteDetailStatusId = dnd.DeliveryNoteDetailStatusId
                                        INNER JOIN [dbo].[Order] AS O
                                            ON dnd.OrderId = O.OrderId 
                                        LEFT JOIN dbo.ClientCarrierTrackingStatus AS c
                                            ON dnd.DriverLastUpdatedStatusId = c.CarrierTrackingStatusId
                                            AND c.ClientId = '{clientId}' 
                                        INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                            ON O.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                            AND cts.ClientId = '{clientId}'
                                        INNER JOIN dbo.OrderAddress AS oa
                                            ON oa.OrderAddressId = O.OrderAddressId
                                        INNER JOIN dbo.PaymentMethodLookup AS PML
                                            ON PML.PaymentMethodId = O.PaymentMethodId ";
      string whereStart = "WHERE ( dnd.Active=1 ";
      string whereEnd = ")";

      #region deliveryNoteId
      if (!string.IsNullOrEmpty(deliveryNoteId))
      {
        dynamicParams.Add("@deliveryNoteId", deliveryNoteId);
        whereStart += "And (dnd.DeliveryNoteId = @deliveryNoteId) ";
      }
      #endregion
      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;

      return result;
    }
  }
  public async Task<DeliveryNoteDetail?> UpdateDeliveryNoteDetail(DeliveryNoteDetail? deliveryNoteDetail)
  {
    _context.DeliveryNoteDetails.Update(deliveryNoteDetail!);
    await _context.SaveChangesAsync();
    return deliveryNoteDetail;
  }

  public async Task<dynamic> GetCompletedDeliveryNoteExpenses(string deliveryNoteId)
  {
    decimal? totalExpense = 0;
    decimal? totalCash = 0;
    decimal? totalAmount = 0;

    var noteId = new DeliveryNoteId(new Guid(deliveryNoteId));
    var receivable = await _context.DriverReceivables.FirstOrDefaultAsync(x => x.DeliveryNoteId == noteId && x.Active == true);
    if (receivable != null)
    {
      totalExpense = receivable.Expense;
      totalCash = receivable.Cash;
      totalAmount = receivable.Total;
    }

    var expenses = await (from e in _context.Expenses
                          join c in _context.ExpenseCategories on e.ExpenseCategoryId equals c.ExpenseCategoryId into cg
                          from c in cg.DefaultIfEmpty()
                          where e.DeliveryNoteId == noteId && e.Active == true
                          select new
                          {
                            ExpenseId = e.ExpenseId != null ? e.ExpenseId.Value.ToString() : null,
                            Amount = e.Amount,
                            ExpenseDate = e.ExpenseDate,
                            ExpenseCategoryId = e.ExpenseCategoryId,
                            ExpenseCategoryName = c != null ? c.ExpenceName : "",
                            Details = e.Details
                          }).ToListAsync();

    dynamic result = new ExpandoObject();
    result.TotalExpense = totalExpense;
    result.TotalCash = totalCash;
    result.TotalAmount = totalAmount;
    result.Expenses = expenses;

    return result;
  }

  public async Task<string> GetClientNextNoteNumber(ClientId? clientId)
  {
    int noteCount = 0;
    int? clientIdentifier = 0;
    var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
    if (client is not null)
    {
      clientIdentifier = client.ClientIdentifier;
      // Count notes specifically for this client to keep sequence clean
      noteCount = await _context.DeliveryNotes.CountAsync(x => x.ClientId == clientId);
      noteCount++;

      var random = new Random();
      int randDigit = random.Next(1, 10); // Generates a random number from 1 to 9
      string candidateNoteNo = $"DN{randDigit}{clientIdentifier}{noteCount}";

      // Loop to guarantee uniqueness and prevent collision
      while (await _context.DeliveryNotes.AnyAsync(dn => dn.NoteNo == candidateNoteNo && dn.ClientId == clientId))
      {
        noteCount++;
        candidateNoteNo = $"DN{randDigit}{clientIdentifier}{noteCount}";
      }
      return candidateNoteNo;
    }
    int totalCount = await _context.DeliveryNotes.CountAsync();
    var fallbackRandom = new Random();
    return $"DN{fallbackRandom.Next(1, 10)}{totalCount + 1}";
  }

  public async Task<dynamic> GetDeliveryNotePaymentInfo(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"";
      string whereStart = "WHERE ( dnd.Active=1 ";
      string whereEnd = ")";
      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data;
    }
  }
  public async Task<dynamic> GetRunSheetInfoByDeliveryNoteId(string deliveryNoteId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @$"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY o.CreatedOn)) AS RowNum,
                                     COUNT(*) OVER () AS TotalCount,
                                     dnd.OrderId,
                                     o.OrderNo,
                                     ISNULL(o.CarrierTrackingNo, '') AS TrackingNo,
                                     oa.CustomerName AS Customer,
                                     oa.CustomerFullAddress AS Address,
                                     o.[Description],
                                     o.Remarks,
                                     e.EmployeeName AS DriverName,
                                     e.MobileNo AS DriverMobile,
                                     ea.FullAddress AS DriverAddress,
                                     pml.Code AS PaymentMethod, 
                                     CASE
                                     WHEN o.PaymentMethodId = {(int)EnumPaymentMethod.PP} THEN
                                         0
                                     ELSE
                                       o.Amount
                                     END AS COD,
                                     ISNULL(c.ClientCompanyName, c.ClientName) AS ClientName,
                                     ISNULL(scc.SaleChannelName, '') AS SaleChannelName
                              FROM dbo.DeliveryNoteDetail AS dnd
                                  INNER JOIN dbo.DeliveryNote AS dn
                                      ON dn.DeliveryNoteId = dnd.DeliveryNoteId
                                  INNER JOIN dbo.[Order] AS o
                                      ON o.OrderId = dnd.OrderId
                                  INNER JOIN dbo.OrderAddress AS oa
                                      ON oa.OrderAddressId = o.OrderAddressId
                                  INNER JOIN dbo.Client AS c
                                      ON o.ClientId = c.ClientId
                                  INNER JOIN dbo.PaymentMethodLookup AS pml
                                      ON pml.PaymentMethodId = o.PaymentMethodId
                                  INNER JOIN dbo.Driver AS d
                                      ON d.DriverId = dn.DriverId
                                  INNER JOIN dbo.Employee AS e
                                      ON e.EmployeeId = d.EmployeeId
                                  INNER JOIN dbo.EmployeeAddress AS ea
                                          ON ea.EmployeeId = d.EmployeeId
                                  LEFT JOIN dbo.SaleChannelConfig AS scc
                                          ON scc.SaleChannelConfigId = o.SaleChannelConfigId ";
      string whereStart = " WHERE ( dnd.Active=1 ";
      string whereEnd = ")";
      //DeliveryNoteId
      if (!string.IsNullOrEmpty(deliveryNoteId))
      {
        dynamicParams.Add("@deliveryNoteId", deliveryNoteId);
        whereStart += "And (dn.DeliveryNoteId = @deliveryNoteId) ";
      }

      string where = whereStart + whereEnd;


      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }
  public async Task<bool> CheckOrderExistsInDND(OrderId? orderId, EmployeeId employeeId)
  {
    var list = await _context.DeliveryNoteDetails.Where(x => x.OrderId == orderId).ToListAsync();
    if (list is not null && list.Count > 0)
    {
      foreach (var item in list)
      {
        item.IsInProcess = false;
        item.UpdatedOn = DateTime.UtcNow;
        item.UpdatedBy = employeeId;
      }
    }
    return true;
  }

  public async Task<dynamic> GetAllDeliveryNoteForDriverById(string? driverId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY dn.NoteNo)) AS RowNum,
                                             COUNT(*) OVER () AS TotalCount,
                                             SubQuery.Pending,
                                             SubQuery.Delivered,
                                             SubQuery.Pending + SubQuery.Delivered AS TotalShipments,
                                             dn.DeliveryNoteId,
                                             dn.NoteNo
                                FROM
                                (
                                    SELECT SUM(   CASE
                                                      WHEN dnd.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Pending}
                                                           OR dnd.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Attempted}
                                                           OR o.CarrierTrackingStatusId <> {(int)EnumCarrierTrackingStatus.Delivered} THEN
                                                          1
                                                      ELSE
                                                          0
                                                  END
                                              ) AS Pending,
                                           SUM(   CASE
                                                      WHEN dnd.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Completed}
                                                           AND o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered} THEN
                                                          1
                                                      ELSE
                                                          0
                                                  END
                                              ) AS Delivered,
                                           dnd.DeliveryNoteId
                                    FROM dbo.DeliveryNote AS dn
                                        INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                            ON dn.DeliveryNoteId = dnd.DeliveryNoteId
                                        INNER JOIN dbo.[Order] AS o
                                            ON o.OrderId = dnd.OrderId
                                        INNER JOIN dbo.Driver AS d
                                            ON d.DriverId = dn.DriverId
                                    WHERE (
                                              dn.Active = 1
                                              AND d.DriverId = '{driverId}'
                                          )
                                    GROUP BY dnd.DeliveryNoteId
                                ) AS SubQuery
                                    INNER JOIN
                                    (SELECT DeliveryNoteId, NoteNo FROM dbo.DeliveryNote) AS dn
                                        ON dn.DeliveryNoteId = SubQuery.DeliveryNoteId ORDER BY dn.NoteNo DESC";

      var data = await connection.QueryAsync(query, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }

  public async Task<dynamic> GetDeliveryNoteDetailForDriverById(string? driverId, string? deliveryNoteId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT
                                ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY dnd.CreatedOn)) AS RowNum,
                                COUNT(*) OVER () AS TotalDeliveryNoteDetail,
									              o.OrderId,
									              o.OrderNo,
									              ISNULL(o.RefNo,'') AS RefNo,
									              o.OrderDate, 
                                CASE
                                   WHEN o.PaymentMethodId = {(int)EnumPaymentMethod.PP} THEN
                                       0
                                   ELSE
                                       o.Amount
                                END AS Amount,
									              pml.Code AS Payment,
									              o.PaymentMethodId,
									              o.CarrierTrackingStatus AS TrackingStatus,
                                ISNULL(o.OrderLabels,'') AS OrderLabels,
									              o.CarrierTrackingStatusId,
									              oa.OrderAddressId,
									              oa.CustomerName,
									              oa.CustomerFullAddress,
                                oa.Latitude,
                                oa.Longitude,
                                oa.Mobile1,
                                oa.Mobile2,
									              dn.DeliveryNoteId,
												        dn.NoteNo,
									              dnd.DeliveryNoteDetailId,
                                dnsl.DeliveryNoteDetailStatusName,
                                dnd.DeliveryNoteDetailStatusId
								                        FROM DeliveryNote AS dn
										                        LEFT JOIN DeliveryNoteDetail AS dnd 
								                        ON dn.DeliveryNoteId=dnd.DeliveryNoteId
                                            LEFT JOIN Driver AS d 
														            ON d.DriverId=dn.DriverId
										                        LEFT JOIN dbo.[Order] AS o 
								                        ON o.OrderId=dnd.OrderId
										                        LEFT JOIN OrderAddress AS oa 
								                        ON oa.OrderAddressId=o.OrderAddressId
										                        LEFT JOIN PaymentMethodLookup AS pml
								                        ON pml.PaymentMethodId=o.PaymentMethodId
                                        INNER JOIN dbo.DeliveryNoteDetailStatusLookup AS dnsl
                                                ON dnsl.DeliveryNoteDetailStatusId = dnd.DeliveryNoteDetailStatusId ";
      string whereStart = "WHERE ( dn.Active=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      #region driverId
      if (!string.IsNullOrEmpty(driverId))
      {
        dynamicParams.Add("@driverId", driverId);
        whereStart += "AND (d.driverId = @driverId) ";
      }
      #endregion
      #region deliveryNoteId
      if (!string.IsNullOrEmpty(deliveryNoteId))
      {
        dynamicParams.Add("@deliveryNoteId", deliveryNoteId);
        whereStart += "AND (dn.deliveryNoteId = @deliveryNoteId) ";
      }
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = query + where;

      var data = await connection.QueryAsync(queryForCount, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalDeliveryNoteDetail;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }
  public async Task<dynamic> GetDriverOrdersByDriverId(string? driverId, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT
                              ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY dnd.CreatedOn)) AS RowNum,
                              COUNT(*) OVER () AS TotalDeliveryNoteDetail,
								              o.OrderId,
								              o.OrderNo,
								              ISNULL(o.RefNo,'') AS RefNo,
								              o.OrderDate, 
                              CASE
                                 WHEN o.PaymentMethodId = {(int)EnumPaymentMethod.PP} THEN
                                     0
                                 ELSE
                                     o.Amount
                              END AS Amount,
								              pml.Code AS Payment,
								              o.PaymentMethodId,
								              o.CarrierTrackingStatus AS TrackingStatus,
								              o.CarrierTrackingStatusId,
								              oa.OrderAddressId,
                              ISNULL(o.OrderLabels,'') AS OrderLabels,
								              oa.CustomerName,
								              oa.CustomerFullAddress,
                              oa.Latitude,
                              oa.Longitude,
								              dn.DeliveryNoteId,
											        dn.NoteNo,
								              dnd.DeliveryNoteDetailId,
                              dnsl.DeliveryNoteDetailStatusName,
                              dnd.DeliveryNoteDetailStatusId
							                        FROM DeliveryNote AS dn
									                        LEFT JOIN DeliveryNoteDetail AS dnd 
							                        ON dn.DeliveryNoteId=dnd.DeliveryNoteId
                                          INNER JOIN Driver AS d 
													            ON d.DriverId=dn.DriverId
									                        LEFT JOIN dbo.[Order] AS o 
							                        ON o.OrderId=dnd.OrderId
									                        LEFT JOIN OrderAddress AS oa 
							                        ON oa.OrderAddressId=o.OrderAddressId
									                        LEFT JOIN PaymentMethodLookup AS pml
							                        ON pml.PaymentMethodId=o.PaymentMethodId
                                      INNER JOIN dbo.DeliveryNoteDetailStatusLookup AS dnsl
                                              ON dnsl.DeliveryNoteDetailStatusId = dnd.DeliveryNoteDetailStatusId ";
      string whereStart = "WHERE ( dn.Active=1 ";
      string whereEnd = ")";
      #region driverId
      if (!string.IsNullOrEmpty(driverId))
      {
        dynamicParams.Add("@driverId", driverId);
        whereStart += "AND (d.driverId = @driverId) ";
      }
      #endregion

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ((o.ClientId in (select value from STRING_SPLIT(@search,',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dnd.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dnd.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;
      string queryForCount = query + where;

      var data = await connection.QueryAsync(queryForCount, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalDeliveryNoteDetail;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }

  public async Task<bool> DeleteDeliveryNoteDetail(DeliveryNoteDetail deliveryNoteDetail)
  {
    _context.DeliveryNoteDetails.Remove(deliveryNoteDetail);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> MergeDeliveryNotes(string targetNoteId, List<string> sourceNoteIds, DateTime? newCreatedDate, EmployeeId employeeId)
  {
    if (string.IsNullOrEmpty(targetNoteId) || sourceNoteIds == null || sourceNoteIds.Count == 0) return false;

    var targetGuid = new Guid(targetNoteId);
    var targetNote = await _context.DeliveryNotes.FirstOrDefaultAsync(x => x.DeliveryNoteId == new DeliveryNoteId(targetGuid) && x.Active == true);
    if (targetNote is null) return false;

    var otherNoteIds = sourceNoteIds
      .Where(id => !string.Equals(id, targetNoteId, StringComparison.OrdinalIgnoreCase))
      .Select(id => new DeliveryNoteId(new Guid(id)))
      .ToList();

    if (otherNoteIds.Count > 0)
    {
      var detailsToMove = await _context.DeliveryNoteDetails
        .Where(dnd => dnd.DeliveryNoteId != null && otherNoteIds.Contains(dnd.DeliveryNoteId))
        .ToListAsync();

      var existingDetails = await _context.DeliveryNoteDetails
        .Where(dnd => dnd.DeliveryNoteId == targetNote.DeliveryNoteId)
        .ToListAsync();

      var groupedIncoming = detailsToMove.GroupBy(d => d.OrderId).ToList();

      foreach (var group in groupedIncoming)
      {
        var orderId = group.Key;
        var incomingDetails = group.ToList();

        var targetDetail = existingDetails.FirstOrDefault(x => x.OrderId == orderId);

        if (targetDetail != null)
        {
          var task = await _context.DeliveryTasks.FirstOrDefaultAsync(t => t.OrderId == orderId);
          if (task != null)
          {
            task.DeliveryNoteDetailId = targetDetail.DeliveryNoteDetailId;
            _context.DeliveryTasks.Update(task);
          }
          _context.DeliveryNoteDetails.RemoveRange(incomingDetails);
        }
        else
        {
          var keepDetail = incomingDetails.First();
          keepDetail.ReassignToDeliveryNote(targetNote.DeliveryNoteId!, employeeId);
          _context.DeliveryNoteDetails.Update(keepDetail);

          var task = await _context.DeliveryTasks.FirstOrDefaultAsync(t => t.OrderId == orderId);
          if (task != null)
          {
            task.DeliveryNoteDetailId = keepDetail.DeliveryNoteDetailId;
            _context.DeliveryTasks.Update(task);
          }

          if (incomingDetails.Count > 1)
          {
            _context.DeliveryNoteDetails.RemoveRange(incomingDetails.Skip(1));
          }
        }
      }

      var otherNotes = await _context.DeliveryNotes
        .Where(dn => dn.DeliveryNoteId != null && otherNoteIds.Contains(dn.DeliveryNoteId))
        .ToListAsync();

      _context.DeliveryNotes.RemoveRange(otherNotes);
    }

    await _context.SaveChangesAsync();

    var totalDetailsCount = await _context.DeliveryNoteDetails
      .CountAsync(dnd => dnd.DeliveryNoteId == targetNote.DeliveryNoteId && dnd.Active == true);

    targetNote.ShipmentCount = totalDetailsCount;
    if (newCreatedDate.HasValue)
    {
      targetNote.CreatedOn = newCreatedDate.Value;
    }
    targetNote.UpdatedBy = employeeId;
    targetNote.UpdatedOn = DateTime.UtcNow;

    _context.DeliveryNotes.Update(targetNote);
    await _context.SaveChangesAsync();
    return true;
  }
  public async Task<DeliveryNoteDetail> AssignOrderToDeliveryNoteAndCleanup(OrderId orderId, DeliveryNoteId targetNoteId, EmployeeId employeeId)
  {
    var details = await _context.DeliveryNoteDetails.Where(x => x.OrderId == orderId).ToListAsync();
    var targetDetail = details.FirstOrDefault(x => x.DeliveryNoteId == targetNoteId);

    var oldNoteIdsToDecrement = new List<DeliveryNoteId>();

    if (targetDetail != null)
    {
      targetDetail.ReassignToDeliveryNote(targetNoteId, employeeId);
      _context.DeliveryNoteDetails.Update(targetDetail);

      var otherDetails = details.Where(dnd => dnd.DeliveryNoteId != targetNoteId).ToList();
      foreach (var d in otherDetails)
      {
        if (d.DeliveryNoteId != null) oldNoteIdsToDecrement.Add(d.DeliveryNoteId);
      }
      _context.DeliveryNoteDetails.RemoveRange(otherDetails);
    }
    else
    {
      var firstDetail = details.FirstOrDefault();
      if (firstDetail != null)
      {
        if (firstDetail.DeliveryNoteId != null)
        {
          oldNoteIdsToDecrement.Add(firstDetail.DeliveryNoteId);
        }

        firstDetail.ReassignToDeliveryNote(targetNoteId, employeeId);
        _context.DeliveryNoteDetails.Update(firstDetail);
        targetDetail = firstDetail;

        var otherDetails = details.Skip(1).ToList();
        foreach (var d in otherDetails)
        {
          if (d.DeliveryNoteId != null) oldNoteIdsToDecrement.Add(d.DeliveryNoteId);
        }
        _context.DeliveryNoteDetails.RemoveRange(otherDetails);
      }
      else
      {
        targetDetail = DeliveryNoteDetail.CreateDeliveryNoteDetail(targetNoteId, orderId, employeeId);
        await _context.DeliveryNoteDetails.AddAsync(targetDetail);
      }
    }

    if (oldNoteIdsToDecrement.Any())
    {
      foreach (var oldNoteId in oldNoteIdsToDecrement)
      {
        var oldNote = await _context.DeliveryNotes.FirstOrDefaultAsync(dn => dn.DeliveryNoteId == oldNoteId);
        if (oldNote != null)
        {
          int count = Math.Max(0, oldNote.ShipmentCount.GetValueOrDefault() - 1);
          if (count == 0)
          {
            _context.DeliveryNotes.Remove(oldNote);
          }
          else
          {
            oldNote.ShipmentCount = count;
            _context.DeliveryNotes.Update(oldNote);
          }
        }
      }
    }

    await _context.SaveChangesAsync();

    // Update DeliveryTask
    var task = await _context.DeliveryTasks.FirstOrDefaultAsync(t => t.OrderId == orderId);
    if (task != null)
    {
      task.DeliveryNoteDetailId = targetDetail.DeliveryNoteDetailId;
      _context.DeliveryTasks.Update(task);
      await _context.SaveChangesAsync();
    }

    return targetDetail;
  }
  #endregion

}
