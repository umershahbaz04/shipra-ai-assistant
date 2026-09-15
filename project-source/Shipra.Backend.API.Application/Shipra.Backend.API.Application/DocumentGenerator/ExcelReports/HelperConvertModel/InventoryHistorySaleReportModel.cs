using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.DocumentGenerator.ExcelReports.HelperConvertModel;
public class InventoryHistorySaleReportModel
{
  public DateTime? Date { get; set; }
  public int? OpeningBalance { get; set; }
  public int? FromTheOrigin { get; set; }
  public int? ManualStockAdjustment { get; set; }
  public int? Rto { get; set; }
  public int? SchedualForDelivery { get; set; }
  public string? SKU { get; set; }
  public int BalanceInStore { get; set; }
  public string? ShipperName { get; set; }

  public static List<InventoryHistorySaleReportModel> ConvertToViewModel(List<ProductStockAdjustmentHistoryByStockId> list)
  {
    var newList = new List<InventoryHistorySaleReportModel>();

    var groupData = list.OrderBy(x => x.CreatedOn).GroupBy(x => x.CreatedOn.GetValueOrDefault().Date);

    foreach (var group in groupData)
    {
      //foreach (var item in group)
      //{

      //}
      var firstRecord = group.FirstOrDefault();
      var lastRecord = group.LastOrDefault();

      var previousQuantity = group.Sum(item => item.PreviousQuantity);
      var newQuantity = group.Sum(item => item.NewQuantity);

      var forRto = group.Where(x => x.TransactionTypeId == (int)InventoryTransactionType.Return).ToList();
      int? sumRtos = 0;
      foreach (var item in forRto)
      {
        int preQty = item.PreviousQuantity.GetValueOrDefault();
        int newQty = item.NewQuantity.GetValueOrDefault();
        sumRtos = sumRtos + (newQty - preQty);
      }

      var forRtoDeleted = new List<ProductStockAdjustmentHistoryByStockId>(); // No direct mapping for RTODeleted, both are Return
      int? sumDelRtos = 0;
      foreach (var item in forRtoDeleted)
      {
        int preQty = item.PreviousQuantity.GetValueOrDefault();
        int newQty = item.NewQuantity.GetValueOrDefault();
        sumDelRtos = sumDelRtos + (newQty - preQty);
      }
      int? rto = sumRtos + (sumDelRtos);

      var forFullfiled = group.Where(x => x.TransactionTypeId == (int)InventoryTransactionType.Dispatch).ToList();
      var forUnFullfiled = group.Where(x => x.TransactionTypeId == (int)InventoryTransactionType.Deallocation).ToList();

      int? sumFullfilled = 0;
      foreach (var item in forFullfiled)
      {
        int preQty = item.PreviousQuantity.GetValueOrDefault();
        int newQty = item.NewQuantity.GetValueOrDefault();
        sumFullfilled = sumFullfilled + (preQty - newQty);
      }


      int? sumUnFullfiled = 0;
      foreach (var item in forUnFullfiled)
      {
        int preQty = item.PreviousQuantity.GetValueOrDefault();
        int newQty = item.NewQuantity.GetValueOrDefault();
        sumUnFullfiled = sumUnFullfiled + (newQty - preQty);
      }
      int? fullfiledRes = sumFullfilled - (sumUnFullfiled);

      var fromOrigion = group.Where(x => x.TransactionTypeId == (int)InventoryTransactionType.Receipt).Sum(item => item.NewQuantity - item.PreviousQuantity);
      var manualAdj = group.Where(x => x.TransactionTypeId == (int)InventoryTransactionType.Adjustment).Sum(item => item.NewQuantity - item.PreviousQuantity);
      InventoryHistorySaleReportModel obj = new InventoryHistorySaleReportModel();
      obj.Date = firstRecord!.CreatedOn;
      obj.OpeningBalance = firstRecord.PreviousQuantity.GetValueOrDefault();
      obj.BalanceInStore = lastRecord!.NewQuantity.GetValueOrDefault();
      obj.Rto = rto;
      obj.SchedualForDelivery = fullfiledRes;
      obj.SKU = firstRecord.SKU;
      obj.ShipperName = firstRecord.StoreName;
      obj.FromTheOrigin = fromOrigion;
      obj.ManualStockAdjustment = manualAdj;

      newList.Add(obj);
    }

    return newList;
  }

}
public class ProductStockAdjustmentHistoryByStockId
{
  public long ProductStockHistorytId { get; set; }
  public int TransactionTypeId { get; set; }
  public string? Reason { get; set; }
  public DateTime? CreatedOn { get; set; }
  public int? PreviousQuantity { get; set; }
  public int? NewQuantity { get; set; }
  public string? Comment { get; set; }
  public string? SKU { get; set; }
  public string? StoreName { get; set; }
}
public class StockHistoryModel
{
  public int? AvailableQty { get; set; }
  public DateTime? LastUpdated { get; set; }
  public List<InventoryHistorySaleReportModel>? newExl { get; set; }
}
