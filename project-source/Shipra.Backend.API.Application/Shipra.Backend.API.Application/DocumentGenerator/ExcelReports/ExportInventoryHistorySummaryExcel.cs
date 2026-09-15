using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Shipra.Backend.API.Application.DocumentGenerator.ExcelReports.HelperConvertModel;
using Shipra.Backend.API.Application.Features.ProductFeatures.Query.DownloadInventoryHistorySummaryExcel;
using Shipra.Backend.API.Application.Helpers;
using Color = System.Drawing.Color;

namespace Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
public class ExportInventoryHistorySummaryExcel
{
  public byte[] ExportToExcelSummary(StockHistoryModel data)
  {
    byte[]? result = null;
    string heading = "Stock Register";
    ExcelExportHelper excelExportHelper = new ExcelExportHelper();
    DataTable dataTable = excelExportHelper.ListToDataTable(data.newExl!);
    //bool showSrNo = true;

    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    using (ExcelPackage package = new ExcelPackage())
    {
      ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format(heading));
      //int startContentRowFrom = 10;
      //string directoryImages = HttpContext.Current.Server.MapPath("~/Resources/");
      //string logoPath = Path.Combine(directoryImages, "logo.png");
      //excelExportHelper.AddImage(workSheet, 0, 0, logoPath);
      //workSheet.Cells[1, 1, 5, 2].Merge = true;
      var SKU = string.Empty;
      var ShipperName = string.Empty;
      if (data.newExl!.Count > 0)
      {
        SKU = data.newExl[0].SKU;
        ShipperName = data.newExl[0].ShipperName;
      }
      //add heading
      // syntax is Cell[fromRow, fromCol, toRow, toCol]
      string bgClr = "#fce4d6";
      workSheet.Cells[1, 1, 2, 6].Value = heading;
      workSheet.Cells[1, 1, 2, 6].Style.Font.Size = 18;
      workSheet.Cells[1, 1, 2, 6].Style.Font.Bold = true;
      workSheet.Cells[1, 1, 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      workSheet.Cells[1, 1, 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

      workSheet.Cells[1, 1, 2, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
      workSheet.Cells[1, 1, 2, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgClr));

      workSheet.Cells[1, 1, 2, 6].Merge = true;

      workSheet.Cells[3, 1, 3, 1].Value = "Store Name :";
      workSheet.Cells[3, 1, 3, 1].Style.Font.Size = 13;
      workSheet.Cells[3, 1, 3, 1].Style.Font.Bold = true;
      //workSheet.Cells[3, 1, 3, 1].Merge = true;

      workSheet.Cells[3, 2, 3, 2].Value = ShipperName;
      workSheet.Cells[3, 2, 3, 2].Style.Font.Size = 12;
      workSheet.Cells[3, 2, 3, 2].Style.Font.Bold = true;
      //workSheet.Cells[3, 2, 3, 2].Merge = true;
      workSheet.Cells[3, 1, 3, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
      workSheet.Cells[3, 1, 3, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgClr));
      workSheet.Cells[3, 1, 3, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[3, 1, 3, 6].Style.Border.Left.Color.SetColor(Color.LightGray);
      workSheet.Cells[3, 1, 3, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[3, 1, 3, 6].Style.Border.Top.Color.SetColor(Color.LightGray);
      workSheet.Cells[3, 1, 3, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[3, 1, 3, 6].Style.Border.Right.Color.SetColor(Color.LightGray);
      workSheet.Cells[3, 1, 3, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[3, 1, 3, 6].Style.Border.Bottom.Color.SetColor(Color.LightGray);

      workSheet.Cells[4, 1, 4, 1].Value = "Product Sku";
      workSheet.Cells[4, 1, 4, 1].Style.Font.Size = 13;
      workSheet.Cells[4, 1, 4, 1].Style.Font.Bold = true;
      workSheet.Cells[4, 1, 4, 1].Merge = true;

      workSheet.Cells[4, 2, 4, 2].Value = SKU;
      workSheet.Cells[4, 2, 4, 2].Style.Font.Size = 12;
      workSheet.Cells[4, 2, 4, 2].Style.Font.Bold = true;
      workSheet.Cells[4, 2, 4, 2].Merge = true;

      workSheet.Cells[4, 5, 4, 6].Value = "In Pcs*****";
      workSheet.Cells[4, 5, 4, 6].Style.Font.Size = 13;
      workSheet.Cells[4, 5, 4, 6].Style.Font.Bold = true;
      workSheet.Cells[4, 5, 4, 6].Merge = true;


      workSheet.Cells[4, 1, 4, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
      workSheet.Cells[4, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgClr));
      workSheet.Cells[4, 1, 4, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[4, 1, 4, 6].Style.Border.Left.Color.SetColor(Color.LightGray);

      workSheet.Cells[5, 1, 5, 1].Value = "Available Quantity";
      workSheet.Cells[5, 1, 5, 1].Style.Font.Size = 13;
      workSheet.Cells[5, 1, 5, 1].Style.Font.Bold = true;
      workSheet.Cells[5, 1, 5, 1].Merge = true;

      workSheet.Cells[5, 2, 5, 2].Value = data.AvailableQty;
      workSheet.Cells[5, 2, 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      workSheet.Cells[5, 2, 5, 2].Style.Font.Size = 12;
      workSheet.Cells[5, 2, 5, 2].Style.Font.Bold = true;
      workSheet.Cells[5, 2, 5, 2].Merge = true;



      workSheet.Cells[5, 5, 5, 5].Value = "Last UpdatedOn";
      workSheet.Cells[5, 5, 5, 5].Style.Font.Size = 13;
      workSheet.Cells[5, 5, 5, 5].Style.Font.Bold = true;
      workSheet.Cells[5, 5, 5, 5].Merge = true;

      workSheet.Cells[5, 6, 5, 6].Value = data.LastUpdated.GetValueOrDefault().ToString("dd/MM/yyyy");
      workSheet.Cells[5, 6, 5, 6].Style.Font.Size = 12;
      workSheet.Cells[5, 6, 5, 6].Style.Font.Bold = true;
      workSheet.Cells[5, 6, 5, 6].Merge = true;


      workSheet.Cells[5, 1, 5, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
      workSheet.Cells[5, 1, 5, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgClr));
      workSheet.Cells[5, 1, 5, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[5, 1, 5, 6].Style.Border.Left.Color.SetColor(Color.LightGray);

      workSheet.Cells[6, 3, 6, 4].Value = "Receipt";
      workSheet.Cells[6, 3, 6, 4].Style.Font.Size = 13;
      workSheet.Cells[6, 3, 6, 4].Style.Font.Bold = true;
      workSheet.Cells[6, 3, 6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      workSheet.Cells[6, 3, 6, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[6, 3, 6, 4].Style.Border.Left.Color.SetColor(Color.Black);
      workSheet.Cells[6, 3, 6, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[6, 3, 6, 4].Style.Border.Right.Color.SetColor(Color.Black);
      workSheet.Cells[6, 3, 6, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[6, 3, 6, 4].Style.Border.Bottom.Color.SetColor(Color.Black);
      workSheet.Cells[6, 3, 6, 4].Merge = true;


      workSheet.Cells[6, 5, 6, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[6, 5, 6, 6].Style.Border.Right.Color.SetColor(Color.Black);

      workSheet.Cells[6, 1, 6, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
      workSheet.Cells[6, 1, 6, 2].Style.Border.Right.Color.SetColor(Color.Black);


      int rowIndex = 7;
      int rowEndTable = data.newExl.Count;
      int colStart = 1;
      int colEnd = 6;
      #region column head
      for (int colIndex = colStart; colIndex <= colEnd; colIndex++)
      {
        string colHead = string.Empty;
        switch (colIndex)
        {
          case 1:
            colHead = "Date";
            break;
          case 2:
            colHead = "Opening Balance";
            break;
          case 3:
            colHead = "From the origin";
            break;
          case 4:
            colHead = "RTO";
            break;
          case 5:
            colHead = "Scheduled for Delivery";
            break;
          case 6:
            colHead = "Balance in Store";
            break;
        }
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Value = colHead;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Font.Size = 13;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Font.Bold = true;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Merge = true;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Border.Right.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, colIndex, rowIndex, colIndex].AutoFitColumns();

        //workSheet.Column(colIndex).AutoFit();
      }

      excelExportHelper.AddBottomLine(workSheet, rowIndex - 2, colStart, colEnd);
      excelExportHelper.AddBottomLine(workSheet, rowIndex, colStart, colEnd);

      #endregion column head

      ++rowIndex;
      foreach (var item in data.newExl)
      {
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Value = item.Date!.Value.ToString("dd/MM/yyyy");
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Merge = true;
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 1, rowIndex, 1].Style.Border.Bottom.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 1, rowIndex, 1].AutoFitColumns();

        workSheet.Cells[rowIndex, 2, rowIndex, 2].Value = item.OpeningBalance;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Merge = true;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 2, rowIndex, 2].AutoFitColumns();

        workSheet.Cells[rowIndex, 3, rowIndex, 3].Value = item.ManualStockAdjustment != 0 ? $"{item.FromTheOrigin} (Manual Adjust {item.ManualStockAdjustment})" : item.FromTheOrigin.GetValueOrDefault().ToString();
        workSheet.Cells[rowIndex, 3, rowIndex, 3].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 3, rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, 3, rowIndex, 3].Merge = true;
        workSheet.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 3, rowIndex, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 2, rowIndex, 2].Style.Border.Bottom.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 3, rowIndex, 3].AutoFitColumns();

        workSheet.Cells[rowIndex, 4, rowIndex, 4].Value = item.Rto;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Merge = true;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 4, rowIndex, 4].Style.Border.Bottom.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 4, rowIndex, 4].AutoFitColumns();

        workSheet.Cells[rowIndex, 5, rowIndex, 5].Value = item.SchedualForDelivery;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Merge = true;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 5, rowIndex, 5].Style.Border.Bottom.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 5, rowIndex, 5].AutoFitColumns();

        workSheet.Cells[rowIndex, 6, rowIndex, 6].Value = item.BalanceInStore;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Font.Size = 12;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Merge = true;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Left.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Bottom.Color.SetColor(Color.Black);
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        workSheet.Cells[rowIndex, 6, rowIndex, 6].Style.Border.Right.Color.SetColor(Color.Black);
        //workSheet.Cells[rowIndex, 6, rowIndex, 6].AutoFitColumns();

        //ROW INCREMENT
        rowIndex++;
      }
      workSheet.Cells[5, 1, 6, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
      workSheet.Cells[5, 1, 6, 6].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(bgClr));
      //workSheet.Cells.AutoFitColumns();
      //rowIndex = rowIndex + shipperInvoiceData.ShipperShipments.Count;
      //excelExportHelper.AddBottomLine(workSheet, rowIndex, colStart, colEnd);
      // add the content into the Excel file
      //workSheet.Cells["A" + startContentRowFrom].LoadFromDataTable(dataTable, true);




      result = package.GetAsByteArray();
    }
    return result;
  }
}
