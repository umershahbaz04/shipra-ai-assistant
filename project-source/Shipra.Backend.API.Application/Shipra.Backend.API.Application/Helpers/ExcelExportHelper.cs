using System.ComponentModel;
using System.Data;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Shipra.Backend.API.Application.Helpers;

public class ExcelExportHelper
{
  public static string ExcelContentType
  {
    get
    { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
  }
  public static string GetExcelFileName(string fileName = "Excel")
  {
    fileName = $"{fileName}_{Guid.NewGuid().ToString().Substring(0, 4)}.xlsx";
    return fileName;
  }

  public DataTable ListToDataTable<T>(List<T> data)
  {
    var properties = TypeDescriptor.GetProperties(typeof(T));
    var dataTable = new DataTable();

    for (var i = 0; i < properties.Count; i++)
    {
      var property = properties[i];
      dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
    }

    var values = new object[properties.Count];
    foreach (var item in data)
    {
      for (var i = 0; i < values.Length; i++)
      {
        values[i] = properties[i].GetValue(item)!;
      }

      dataTable.Rows.Add(values);
    }
    return dataTable;
  }

  public DataTable DynamicListToDataTable(dynamic data)
  {
    var json = JsonConvert.SerializeObject(data);
    DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)))!;
    return dataTable;
  }


  public DataTable ListToDataTableV2(List<string> headers, List<List<string>> data)
  {
    var dataTable = new DataTable();
    object[] values;
    for (var i = 0; i < headers.Count; i++)
    {
      dataTable.Columns.Add(headers[i]);
    }
    if (data.Count > 0)
    {
      values = new object[data[0].Count];
      for (var k = 0; k < data.Count; k++)
      {
        var rd = data[k];
        for (var i = 0; i < values.Length; i++)
        {
          values[i] = rd[i];
        }

        dataTable.Rows.Add(values);
      }
    }
    return dataTable;
  }

  public int Pixel2MTU(int pixels)
  {
    var mtus = pixels * 9525;
    return mtus;
  }
  public void AddTopLine(ExcelWorksheet oSheet, int rowIndex, int startColumnIndex, int endColumnIndex)
  {

    for (var i = startColumnIndex; i <= endColumnIndex; i++)
    {
      var cell = oSheet.Cells[rowIndex, i++];

      //Setting top,left,right,bottom border of header cells
      var border = cell.Style.Border;
      border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
    }

  }
  public void AddBottomLine(ExcelWorksheet oSheet, int rowNumber, int startColumnIndex, int endColumnIndex)
  {

    for (var i = startColumnIndex; i <= endColumnIndex; i++)
    {
      var cell = oSheet.Cells[rowNumber, i];

      //Setting top,left,right,bottom border of header cells
      var border = cell.Style.Border;
      border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
    }

  }

}
