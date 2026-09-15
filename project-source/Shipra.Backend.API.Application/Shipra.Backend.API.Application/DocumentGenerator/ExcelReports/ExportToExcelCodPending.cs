using System.Data;
using OfficeOpenXml;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;
public class ExportToExcelCodPending
{
  public byte[] ExportToExcel(dynamic data, string title)
  {
    byte[]? result = null;
    string heading = title;
    ExcelExportHelper excelExportHelper = new ExcelExportHelper();

    #region No Data Found and converted data
    bool IsDataEmpty = false;
    if (data.Count == 0)
    {
      IsDataEmpty = true;
      data = new List<dynamic> { new { Message = "No Record Found" } };
    }

    DataTable dataTable = excelExportHelper.DynamicListToDataTable(data);
    if (!IsDataEmpty)
    {
      dataTable.Columns.Remove("OrderId");
    }
    #endregion

    //string[] columnsToTake = { "Reason", "SKU", "PreviousQuantity", "NewQuantity", "CreatedOn", "Comment" };
    bool showSrNo = true;
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    using (ExcelPackage package = new ExcelPackage())
    {
      ExcelWorksheet workSheet = package.Workbook.Worksheets.Add(String.Format(heading));
      int startContentRowFrom = 1;
      for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
      {
        //add Sr# column
        if (i == 0 && showSrNo)
        {
          DataColumn dataColumn = dataTable.Columns.Add("Sr #", typeof(int));
          dataColumn.SetOrdinal(0);
          int index = 1;
          foreach (DataRow item in dataTable.Rows)
          {
            item[0] = index;
            index++;
          }
          continue;
        }
        //set column caption
        //if (dataTable.Columns[i].ColumnName.Equals("Key")) dataTable.Columns[i].Caption = "Key"; 

        // removed ignored columns
        //if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
        //{
        //  dataTable.Columns.RemoveAt(i);
        //  //workSheet.DeleteColumn(i + 1);
        //}
      }
      // add the content into the Excel file
      workSheet.Cells["A" + startContentRowFrom].LoadFromDataTable(dataTable, true);

      // autofit width of cells with small content
      int columnIndex = 1;
      foreach (DataColumn column in dataTable.Columns)
      {
        ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];
        int maxLength = columnCells.Max(cell => cell.Value != null ? cell.Value.ToString()!.Count() : 0);
        if (maxLength < 150)
        {
          workSheet.Column(columnIndex).AutoFit();
        }
        if (column.Caption.Contains("Date") || column.Caption.Contains("CreatedOn") || column.Caption.Contains("UpdatedOn"))
        {
          workSheet.Column(columnIndex).Style.Numberformat.Format = "dd-MMM-yyyy";
        }
        columnIndex++;
      }

      //Sr# column width
      workSheet.Column(1).Width = 5;
      //date format
      //workSheet.Column(7).Style.Numberformat.Format = "dd-MMM-yyyy";

      // format column content header - bold, blue on black
      using (ExcelRange r = workSheet.Cells[startContentRowFrom, 1, startContentRowFrom, dataTable.Columns.Count])
      {
        //r.Style.Font.Color.SetColor(System.Drawing.Color.White);
        r.Style.Font.Bold = true;
        //r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#1fb5ad"));
      }

      //add bottom line
      //excelExportHelper.AddBottomLine(workSheet, 9, 1, 7);
      //excelExportHelper.AddBottomLine(workSheet, dataTable.Rows.Count + 11, 1, 7);
      result = package.GetAsByteArray();
    }
    return result!;
  }

}
