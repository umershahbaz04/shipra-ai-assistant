using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class OrderTrackingHistoryRepository : IOrderTrackingHistoryRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public OrderTrackingHistoryRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<OrderTrackingHistory> CreateOrderTrackingHistory(OrderTrackingHistory orderHistory)
  {
    await _context.OrderTrackingHistories.AddAsync(orderHistory!);
    await _context.SaveChangesAsync();
    return orderHistory!;
  }
  public async Task<OrderNote> CreateOrderNote(OrderNote orderNote)
  {
    await _context.OrderNotes.AddAsync(orderNote!);
    await _context.SaveChangesAsync();
    return orderNote!;
  }

  public async Task<OrderNote> GetOrderNoteById(OrderNoteId orderNoteId)
  {
    var data = await _context.OrderNotes.FirstOrDefaultAsync(x => x.OrderNoteId == orderNoteId);
    return data!;
  }

  public async Task<bool> DeleteOrderById(OrderNote orderNote)
  {
    _context.Remove(orderNote);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic?> GetOrderNoteByOrderNo(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT orn.NoteDescription,
                               e.EmployeeName AS CreatedByName,
                               orn.CreatedOn
                        FROM dbo.[OrderNote] AS orn
                            INNER JOIN dbo.[Order] AS o
                                ON o.OrderId = orn.OrderId
                            INNER JOIN dbo.Employee AS e
                                ON e.EmployeeId = orn.CreatedBy ";
      string whereStart = " WHERE ( ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " o.ClientId = @ClientId ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And o.OrderNo = @orderNo ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where + "ORDER BY orn.CreatedOn DESC ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<List<OrderTrackingHistory>?> GetOrderTrackingHistoryByOrderId(OrderId orderId)
  {
    var data = await _context.OrderTrackingHistories.Where(x => x.OrderId == orderId).ToListAsync();
    return data;
  }
  public async Task<bool> DeleteOrderTrackingHistory(List<OrderTrackingHistory> orderTrackingHistories)
  {
    _context.OrderTrackingHistories.RemoveRange(orderTrackingHistories);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic?> GetOrderTrackingHistoryByOrderNo(string orderNo, string clientId)
  {
    orderNo = orderNo.Trim();
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT cts.TrackingStatus,
                               ot.TrackingStatusComments,
                               ot.Latitude,
                               ot.Longitude,
                               ot.Location,
                               s.StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo, 
                               ot.CreatedByName,
                               ot.CreatedOn,
                               ot.OrderHistoryTypeId,
                               ISNULL(o.CarrierAssignDate,o.OrderDate) AS CarrierAssignDate  
                        FROM dbo.[OrderTrackingHistory] AS ot
                            INNER JOIN dbo.[Order] AS o
                                ON ot.OrderId = o.OrderId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId 
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON ot.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.Employee AS e
                                ON e.EmployeeId = ot.CreatedBy
                            INNER JOIN dbo.OrderHistoryTypeLookup AS ohtl
                                ON ohtl.OrderHistoryTypeId = ot.OrderHistoryTypeId ";

      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "o.ClientId = @ClientId ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And o.OrderNo = @orderNo OR o.CarrierTrackingNo = @orderNo ";
      }
      string where = whereStart + whereEnd;
      string groupBy = @" GROUP BY ISNULL(o.CarrierAssignDate, o.OrderDate),
                 cts.TrackingStatus,
                 ot.TrackingStatusComments,
                 ot.Latitude,
                 ot.Longitude,
                 ot.Location,
                 s.StoreName,
                 s.StoreImage,
                 s.CustomerServiceNo,
                 ot.CreatedByName,
                 ot.CreatedOn,
                 ot.OrderHistoryTypeId ";
      string queryData = query + where + " " + groupBy + "ORDER BY ot.CreatedOn DESC ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<dynamic?> GetOrderTrackingHistoryByOrderNoForView(string orderNo, string clientId)
  {
    orderNo = orderNo.Trim();

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT ot.CarrierTrackingStatusId as TrackingStatusId,
                               cts.TrackingStatus,
                               ot.TrackingStatusComments, 
                               s.StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo ,
                               ot.CreatedOn 
                        FROM dbo.[OrderTrackingHistory] AS ot
                            INNER JOIN dbo.[Order] AS o
                                ON ot.OrderId = o.OrderId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON ot.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}' 
                            INNER JOIN dbo.OrderHistoryTypeLookup AS ohtl
                                ON ohtl.OrderHistoryTypeId = ot.OrderHistoryTypeId  ";
      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "(o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And (o.OrderNo = @orderNo) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY ot.CreatedOn desc ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<OrderNote> UpdateOrderNote(OrderNote orderNote)
  {
    _context.OrderNotes.Update(orderNote);
    await _context.SaveChangesAsync();
    return orderNote!;
  }

  public async Task<OrderNote?> GetOrderNoteByOrderId(OrderId orderId)
  {
    return await _context.OrderNotes.FirstOrDefaultAsync(x => x.OrderId == orderId);
  }
}
