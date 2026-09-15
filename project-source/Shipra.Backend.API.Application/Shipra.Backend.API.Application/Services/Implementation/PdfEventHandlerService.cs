using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using iText.Kernel.Pdf;
using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element; 
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;
using DocumentFormat.OpenXml.Presentation;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class PdfEventHandler : IEventHandler
{
  private readonly string _footerContent; 

  public PdfEventHandler(string footerContent)
  {
    _footerContent = footerContent; 
  }

  public void HandleEvent(Event currentEvent)
  {
    PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
    PdfDocument pdfDoc = docEvent.GetDocument();
    PdfPage page = docEvent.GetPage();
    PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
    Rectangle pageSize = page.GetPageSize();

    // Add top border 
    pdfCanvas.MoveTo(pageSize.GetLeft(), pageSize.GetBottom() + 25)
             .LineTo(pageSize.GetRight(), pageSize.GetBottom() + 25)
             .Stroke();

    // Create a font program from a font file
    PdfFont font = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", PdfEncodings.IDENTITY_H);

    // Add footer content
    // Get the width of the footer content
    float textWidth = font.GetWidth(_footerContent, 12);

    // Set padding
    float padding = 5; // Adjust padding value as needed

    // Calculate the x-coordinate with padding
    float xCoordinate = pageSize.GetRight() - textWidth - padding;

    //Add footer content aligned to the right
    pdfCanvas.BeginText()
             .SetFontAndSize(font, 10)
             .MoveText(xCoordinate, pageSize.GetBottom() + 7)
             .ShowText(_footerContent)
             .EndText();

    pdfCanvas.Release();
  }
}
