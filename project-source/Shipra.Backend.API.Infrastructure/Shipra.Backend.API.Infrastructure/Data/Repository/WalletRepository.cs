using System.Dynamic;
using Ardalis.Result;
using AutoMapper.Execution;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class WalletRepository : IWalletRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _appDbContext;
  private readonly IDbContextService _dbContextService;

  public WalletRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext, AppDbContext appDbContext, IDbContextService dbContextService)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
    _appDbContext = appDbContext;
    _dbContextService = dbContextService;
  }
  public async Task<bool> CreatePaymentLink(PaymentLink paymentLink)
  {
    await _appDbContext.PaymentLinks.AddAsync(paymentLink);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<PaymentLink?> GetPaymentLinkByOrderId(OrderId orderId)
  {
    return await _appDbContext.PaymentLinks.FirstOrDefaultAsync(x => x.OrderId == orderId);
  }
  public async Task<List<PaymentLinkStatusLookup>?> GetAllPaymentLinkStatusLookup()
  {
    return await _appDbContext.PaymentLinkStatusLookups.ToListAsync();
  }

  public async Task<Wallet?> GetWalletByClientId(ClientId clientId)
  {
    var clid = clientId!.Value.ToString();

    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _appDbContext.Wallets.FirstOrDefaultAsync(x => x.ClientId == clientId);
    }
  }

  public async Task<bool> CreateWallat(Wallet oWallet)
  {
    var clientId = oWallet!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _appDbContext.Wallets.AddAsync(oWallet);
      return await _appDbContext.SaveChangesAsync() > 0;
    }
  }

  public async Task<bool> UpdateWallet(Wallet oWallet)
  {
    var clientId = oWallet!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      _appDbContext.Wallets.Update(oWallet);
      return await _appDbContext.SaveChangesAsync() > 0;
    }
  }

  public async Task<ClientPayoutBank?> GetClientPayoutBank(ClientId clientId)
  {
    return await _appDbContext.ClientPayoutBanks.FirstOrDefaultAsync(x => x.ClientId == clientId);
  }

  public async Task<bool> CreateClientPayoutBank(ClientPayoutBank oWallet)
  {
    await _appDbContext.ClientPayoutBanks.AddAsync(oWallet);
    return await _appDbContext.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllOrdersByPayoutId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string sortDir, string clientId,string? payoutId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               ISNULL(o.RefNo, '') AS RefNo,
                               ISNULL(o.ItemsCount, 0) AS NumberOfPieces,
                               o.OrderDate,
                               o.CreatedOn,
                               pl.Amount, 
                               ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,  
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks, 
                               o.Weight,  
                               ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1 
                        FROM dbo.PaymentLink AS pl
                            INNER JOIN dbo.[Order] AS o
                                ON o.OrderId = pl.OrderId
                            INNER JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.OrderDeliveryType AS odt
                                ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId
                            INNER JOIN dbo.Client AS cl
                                ON cl.ClientId = o.ClientId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @"And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( o.RefNo in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( o.CarrierTrackingNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      #region MyRegion
      dynamicParams.Add("@PayoutId", payoutId);
      whereStart += "And (pl.PayoutId = @PayoutId) ";
      #endregion
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(o.OrderDate AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(o.OrderDate AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      /////////order fileters////
       
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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

  public async Task<bool> UpdateClientPayoutBank(ClientPayoutBank oWallet)
  {
    _appDbContext.ClientPayoutBanks.Update(oWallet);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetAllClientPayoutBank(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT cpb.ClientPayoutBankId,
                             cpb.BankName,
                             cpb.AccountTitle,
                             cpb.IBAN,
                             cpb.SwiftCode,
                             cpb.BranchName
                      FROM dbo.ClientPayoutBank AS cpb ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (cpb.ClientId = @ClientId) ";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.AsList();
    }

  }

  public async Task<dynamic> GetAllClientWallets(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT w.WalletId,
                         w.ClientId,
                         w.AvailableBalance,
                         w.CurrentBalance
                  FROM dbo.Wallet AS w ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (w.ClientId = @ClientId) ";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.AsList();
    }

  }
  public async Task<dynamic> GetAllPayouts(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? payoutStatusId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               p.PayoutId,
                               p.PayoutRef,
                               p.TransactionRef,
                               p.CreatedOn,
                               p.BankName,
                               p.AccountTitle,
                               p.IBAN,
                               p.SwiftCode,
                               p.BranchName,
                               psl.StatusName,
                               p.Currency,
                               ISNULL(p.Amount, 0) AS Amount,
                               ISNULL(p.ServiceCharges, 0) AS ServiceCharges,
                               ISNULL(p.TransactionCharges, 0) AS TransactionCharges,
                               ISNULL(p.Outstanding, 0) AS Outstanding
                        FROM dbo.Payout AS p
                            INNER JOIN dbo.PaymentLink AS pl
                                ON pl.PayoutId = p.PayoutId
                            INNER JOIN dbo.PayoutStatusLookup AS psl
                                ON psl.PayoutStatusId = p.PayoutStatusId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @"And ( ( p.PayoutRef in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( p.TransactionRef in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (p.ClientId = @ClientId) ";

      if (payoutStatusId > 0)
      {
        dynamicParams.Add("@payoutStatusId", payoutStatusId);
        whereStart += $"And (p.PayoutStatusId = @payoutStatusId)  ";
      }
      string groupBy = @" GROUP BY p.PayoutId,
                                             p.BankName,
                                             p.AccountTitle,
                                             p.IBAN,
                                             p.SwiftCode,
                                             p.BranchName,
                                             p.PayoutRef,
                                             p.Currency,
                                             p.Amount,
                                             p.TransactionCharges,
                                             p.ServiceCharges,
                                             p.Outstanding,
                                             p.TransactionRef,
                                             p.CreatedOn,
                                             p.ClientId,
                                             psl.StatusName ";
      string where = whereStart + whereEnd + groupBy + " ORDER BY p.CreatedOn DESC ";
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
  public async Task<dynamic> GetAllTransaction(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? transactionTypeId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               t.TransactionId,
                               ttl.TransactionName,
                               t.TransactionNo,
                               t.Description,
                               t.WalletId,
                               t.Debit,
                               t.Credit,
                               t.CreatedOn,
                               t.TransactionTypeId
                        FROM dbo.Wallet AS w
                            INNER JOIN dbo.[Transaction] AS t
                                ON t.WalletId = w.WalletId
                            INNER JOIN dbo.TransactionTypeLookup AS ttl
                                ON ttl.TransactionTypeId = t.TransactionTypeId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @"And ( ( t.TransactionNo in (select value from STRING_SPLIT(@Search,','))))  ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("t.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("t.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (w.ClientId = @ClientId) ";

      if (transactionTypeId > 0)
      {
        dynamicParams.Add("@transactionTypeId", transactionTypeId);
        whereStart += $"And (t.TransactionTypeId = @transactionTypeId)  ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where + " ORDER BY t.CreatedOn DESC ";
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
  public async Task<dynamic> GetAllPayoutStatusHistory(string clientId, string? payoutId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               psh.PayoutStatusHistoryId,
                               psh.PayoutStatusId,
                               psl.StatusName,
                               psh.Comment,
                               psh.CreatedOn,
                               psh.CreatedBy
                        FROM dbo.PayoutStatusHistory AS psh
                            INNER JOIN dbo.Payout AS p
                                ON p.PayoutId = psh.PayoutId
                            LEFT JOIN dbo.PayoutStatusLookup AS psl
                                ON psl.PayoutStatusId = psh.PayoutStatusId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      dynamicParams.Add("@payoutId", payoutId);
      whereStart += "And (psh.PayoutId = @payoutId) ";


      dynamicParams.Add("@clientId", clientId);
      whereStart += "And (p.ClientId = @clientId) ";

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

  public async Task<bool> UpdatePaymentLink(PaymentLink paymentLink)
  {
    _appDbContext.PaymentLinks.Update(paymentLink);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreateTransaction(Transaction transaction)
  {
    await _appDbContext.Transactions.AddAsync(transaction);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreatePayout(Payout payout)
  {
    await _appDbContext.Payouts.AddAsync(payout);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<PaymentLink?> GetPaymentLinkByPaymentLinkId(PaymentLinkId paymentLinkId)
  {
    return await _appDbContext.PaymentLinks.FirstOrDefaultAsync(x => x.PaymentLinkId == paymentLinkId);
  }

  public async Task<bool> CreatePayoutStatusHistory(PayoutStatusHistory payoutStatusHistory)
  {
    await _appDbContext.PayoutStatusHistories.AddAsync(payoutStatusHistory);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdatePayout(Payout payout)
  {
    _appDbContext.Payouts.Update(payout);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<List<PaymentLink>> GetPaymentLinksByPayoutId(string? payoutId, ClientId? clientId)
  {
    var pOut = new PayoutId(new Guid(payoutId!));
    return await _appDbContext.PaymentLinks.Where(x => x.PayoutId == pOut && x.ClientId == clientId).ToListAsync();

  }

  public async Task<Payout?> GetPayoutById(string? payoutId, ClientId? clientId)
  {
    var pOut = new PayoutId(new Guid(payoutId!));

    return await _appDbContext.Payouts.FirstOrDefaultAsync(x => x.PayoutId == pOut && x.ClientId == clientId);
  }
  public async Task<Payout> CheckTransactionRef(string? transactionRef, ClientId? clientId)
  {
    var data = await _appDbContext.Payouts.FirstOrDefaultAsync(x => x.TransactionRef!.ToLower().Trim() == transactionRef!.ToLower().Trim() && x.ClientId == clientId);
    return data!;
  }

  public async Task<bool> CreatePayoutFile(PayoutFile payoutFile)
  {
    await _appDbContext.PayoutFiles.AddAsync(payoutFile);
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<List<PayoutFile>> GetAllPayoutFiles(PayoutId payoutId, ClientId clientId)
  {
    var result = await _context.PayoutFiles
    .Join(
        _context.Payouts,
        payoutFile => payoutFile.PayoutId, // Outer key selector (PayoutFiles.PayoutId)
        payout => payout.PayoutId,        // Inner key selector (Payout.PayoutId)
        (payoutFile, payout) => new { PayoutFile = payoutFile, Payout = payout } // Result selector
    )
    .Where(joined => joined.Payout.ClientId == clientId && joined.PayoutFile.PayoutId == payoutId) // Filter by ClientId
    .Select(joined => joined.PayoutFile) // Select only the PayoutFile
    .ToListAsync();

    return result;
  }

  public async Task<dynamic> GetCalucaltedBalance(string? clientIdStr)
  {
    var oWallet = await GetWalletByClientId(new ClientId(new Guid(clientIdStr!)));

    //var currentBalance = await _context.Transactions
    //  .GroupBy(t => 1) // Group all rows into a single group
    //  .Select(g => g.Sum(t => t.Credit) - g.Sum(t => t.Debit))
    //  .FirstOrDefaultAsync();


    var avaiableBalance = await _context.PaymentLinks
    .Where(pl => pl.PayoutId == null && pl.PaymentLinkStatusId == (int)EnumPaymentLinkStatus.Paid).SumAsync(x => x.Amount);
     
    ClientId clientId = new ClientId(new Guid(clientIdStr!));

    // Predefined groups to mimic the SQL WITH clause
    var groups = new[]
    {
    new { IsCompleted = true },
    new { IsCompleted = false }
};

    // Fetch the data and perform grouping
    var payouts = await _context.Payouts
        .Where(p => p.ClientId == clientId)
        .ToListAsync(); // Materialize the data to work with LINQ to Objects

    var payoutSums = groups
        .GroupJoin(
            payouts,
            g => g.IsCompleted,
            p => p.PayoutStatusId == (int)EnumPayoutStatus.Completed, // Match completed and not completed
            (g, matchingPayouts) => new
            {
              IsCompleted = g.IsCompleted,
              TotalAmount = matchingPayouts.Sum(p => p.Amount ?? 0) // Handle null amounts
            })
        .ToList();

    // Extract totals
    var completedPayoutSum = payoutSums.FirstOrDefault(g => g.IsCompleted)?.TotalAmount ?? 0;
    var requestedPayoutSum = payoutSums.FirstOrDefault(g => !g.IsCompleted)?.TotalAmount ?? 0;


    dynamic result = new
    {
      WalletId = oWallet!.WalletId!.Value!.ToString(),
      oWallet.CurrentBalance,
      availableBalance = avaiableBalance,
      totalCompletedPayout = completedPayoutSum,
      totalRequestedPayout = requestedPayoutSum
    };


    return result;
  }
}
