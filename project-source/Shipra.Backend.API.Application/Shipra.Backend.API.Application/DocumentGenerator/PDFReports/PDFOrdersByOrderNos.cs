using System.Text.Json;
using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Shipra.Backend.API.Application.DocumentGenerator.PDFReports;
public class PDFOrdersByOrderNos
{
  public async Task<Table> GetPdfTable(dynamic data)
  {
    // Table
    Table table = new Table(4, false);

    // Headings
    Cell cellProductId = new Cell(1, 1)
       .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
       .SetTextAlignment(TextAlignment.CENTER)
       .Add(new Paragraph("Product ID"));

    Cell cellProductName = new Cell(1, 1)
       .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
       .SetTextAlignment(TextAlignment.LEFT)
       .Add(new Paragraph("Product Name"));

    Cell cellQuantity = new Cell(1, 1)
       .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
       .SetTextAlignment(TextAlignment.CENTER)
       .Add(new Paragraph("Quantity"));

    Cell cellUnitPrice = new Cell(1, 1)
       .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
       .SetTextAlignment(TextAlignment.CENTER)
       .Add(new Paragraph("Unit Price"));

    table.AddCell(cellProductId);
    table.AddCell(cellProductName);
    table.AddCell(cellQuantity);
    table.AddCell(cellUnitPrice);

    dynamic products = await GetProductsAsync(data);

    foreach (var item in products)
    {
      Cell cId = new Cell(1, 1)
          .SetTextAlignment(TextAlignment.CENTER)
          .Add(new Paragraph(item.OrderNo.ToString()));

      Cell cName = new Cell(1, 1)
          .SetTextAlignment(TextAlignment.LEFT)
          .Add(new Paragraph(item.Address));

      Cell cQty = new Cell(1, 1)
          .SetTextAlignment(TextAlignment.RIGHT)
          .Add(new Paragraph(item.OrderDate.ToString()));

      Cell cPrice = new Cell(1, 1)
          .SetTextAlignment(TextAlignment.RIGHT)
          .Add(new Paragraph(String.Format("{0:C2}", item.Amount)));

      table.AddCell(cId);
      table.AddCell(cName);
      table.AddCell(cQty);
      table.AddCell(cPrice);
    }

    return table;
  }

  private async Task<dynamic> GetProductsAsync(dynamic data)
  {
    HttpClient client = new HttpClient();
    var stream = client.GetStreamAsync(data);
    var products = await JsonSerializer.DeserializeAsync<dynamic>(await stream);

    return products!;
  }
}
