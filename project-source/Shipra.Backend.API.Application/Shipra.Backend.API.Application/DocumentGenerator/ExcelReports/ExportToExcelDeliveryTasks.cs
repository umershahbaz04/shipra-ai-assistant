using System.Data;
using System.Linq;
using OfficeOpenXml;
using Shipra.Backend.API.Application.Helpers;

namespace Shipra.Backend.API.Application.DocumentGenerator.ExcelReports;

public class ExportToExcelDeliveryTasks
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
      if (dataTable.Columns.Contains("OrderId")) dataTable.Columns.Remove("OrderId");
      if (dataTable.Columns.Contains("RowNum")) dataTable.Columns.Remove("RowNum");
      if (dataTable.Columns.Contains("TotalCount")) dataTable.Columns.Remove("TotalCount");
    }
    #endregion

    string[] columnsToTake = { "OrderNo", "TrackingNo", "RefNo", "Customer", "CustomerFullAddress", "DriverName", "DriverMobile", "DeliveryTaskStatus", "TrackingStatus", "OrderDate", "Amount", "SalePersonName", "Description", "Mobile1", "Mobile2" };
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

        // removed ignored columns
        if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName, System.StringComparer.OrdinalIgnoreCase))
        {
          dataTable.Columns.RemoveAt(i);
        }
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
        if (column.Caption.Contains("Date"))
        {
          workSheet.Column(columnIndex).Style.Numberformat.Format = "dd-MMM-yyyy";
        }
        columnIndex++;
      }

      //Sr# column width
      workSheet.Column(1).Width = 5;

      // format column content header - bold, blue on black
      using (ExcelRange r = workSheet.Cells[startContentRowFrom, 1, startContentRowFrom, dataTable.Columns.Count])
      {
        r.Style.Font.Bold = true;
      }

      result = package.GetAsByteArray();
    }
    return result!;
  }
}
