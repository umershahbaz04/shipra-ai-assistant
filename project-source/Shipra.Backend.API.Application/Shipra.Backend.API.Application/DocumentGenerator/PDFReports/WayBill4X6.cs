using DocumentFormat.OpenXml.Office2010.PowerPoint;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Color = iText.Kernel.Colors.Color;
using Image = iText.Layout.Element.Image;

namespace Shipra.Backend.API.Application.DocumentGenerator.PDFReports;
public class WayBill4X6
{
  Paragraph? el;
  Style? arial7;
  Style? arial7Bold;
  Paragraph? arial8;
  Paragraph? arial8Bold;
  Style? arial9;
  Style? arial9Bold;
  Paragraph? arial11;
  Paragraph? arial11Bold;
  Style? arialHeader;
  Paragraph? arialHeaderWhite;
  dynamic? oWayBillWrappersList = null;

  public WayBill4X6(IEnumerable<dynamic>? wayBillWrappersList)
  {
    oWayBillWrappersList = wayBillWrappersList;
    string fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\arial.ttf";

    // Create a Color object for the font color
    Color fontColor = new DeviceRgb(255, 0, 0); // Red color


    // Create a PdfFont with the desired font
    PdfFont baseFont = PdfFontFactory.CreateFont(fontPath);


    el = new Paragraph().SetFont(baseFont).SetFontSize(11).SetFontColor(fontColor);
    arial8 = new Paragraph().SetFont(baseFont).SetFontSize(8).SetFontColor(fontColor);
    arial8Bold = new Paragraph().SetFont(baseFont).SetFontSize(8).SetFontColor(fontColor).SetBold();
    arial11 = new Paragraph().SetFont(baseFont).SetFontSize(11).SetFontColor(fontColor);
    arial11Bold = new Paragraph().SetFont(baseFont).SetFontSize(11).SetFontColor(fontColor).SetBold();
    //arialHeader = new Paragraph().SetFont(baseFont).SetFontSize(16).SetFontColor(fontColor).SetBold();

    arial7 = new Style().SetFont(baseFont).SetFontSize(7).SetFontColor(fontColor);

    arialHeader = new Style().SetFont(baseFont).SetFontSize(16).SetFontColor(fontColor);
    arial9 = new Style().SetFont(baseFont).SetFontSize(9).SetFontColor(fontColor);
    arial9Bold = new Style().SetFont(baseFont).SetFontSize(9).SetFontColor(fontColor).SetBold();
    arial7Bold = new Style().SetFont(baseFont).SetFontSize(7).SetFontColor(fontColor).SetBold();
    arialHeaderWhite = new Paragraph().SetFont(baseFont).SetFontSize(11).SetFontColor(fontColor);

  }
  public string GenerateAirWayBillPdf()
  {
    try
    {
      string fileName = string.Format(Guid.NewGuid().ToString() + ".pdf");
      // File will be created in this path
      string completeFilePath = Path.Combine(Path.GetTempPath(), fileName);

      Image? image = null;

      // Create PDF Table
      using (FileStream fileStream = new FileStream(completeFilePath, FileMode.Create))
      using (PdfWriter writer = new PdfWriter(fileStream))
      using (PdfDocument pdfDocument = new PdfDocument(writer))
      using (Document document = new Document(pdfDocument, iText.Kernel.Geom.PageSize.A4, true))
      {
        //var castedVawBillData = (IEnumerable<dynamic>)oWayBillWrappersList!;
        foreach (var item in oWayBillWrappersList!)
        {
          if (item != null)
          {
            //if (item.ShipmentItemData != null && item.ShipmentItemData.Count > 0)
            if (false)
            {
              //var TotalCount = item.ShipmentItemData.Count;
              //foreach (var oShipmentItem in item.ShipmentItemData)
              //{
              //  document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
              //  // Add Data
              //  AddData(image,document, item.AirwayBillData, oShipmentItem, TotalCount);
              //}
            }
            else
            {
              // Add Data
              AddData(image, document, item);
              document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }
          }
        }
      }

      return completeFilePath;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return "";
    }
  }

  protected void AddData(Image image, Document document, dynamic data)
  {
    #region Border
    Border solidBorder = new SolidBorder(ColorConstants.BLACK, 0.5f);
    var solidBorderOuter = new SolidBorder(ColorConstants.BLUE, 1);
    var styleTopBorder = new Style().SetBorderTop(solidBorder).SetBorderRight(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER);
    var styleTopRightBorder = new Style().SetBorderTop(solidBorder).SetBorderRight(solidBorder).SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER);
    var styleRightBorder = new Style().SetBorderTop(Border.NO_BORDER).SetBorderRight(solidBorder).SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER);
    var styleBottomBorder = new Style().SetBorderTop(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderBottom(solidBorder).SetBorderLeft(Border.NO_BORDER);
    var styleLeftBorder = new Style().SetBorderTop(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER).SetBorderLeft(solidBorder);
    var styleNoBorder = new Style().SetBorderTop(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER).SetBorderLeft(Border.NO_BORDER);
    #endregion


    Table outertable = new Table(UnitValue.CreatePercentArray(new float[] { 96f, 4f })).UseAllAvailableWidth();
    outertable.SetPadding(0);
    //outertable.SetMargins(0f, 0f, 0f, 0f);

    Table innnertable = new Table(UnitValue.CreatePercentArray(new float[] { 100f })).UseAllAvailableWidth();
    innnertable.SetPadding(0);
    innnertable.SetMargin(0);

    Table firsttable = new Table(UnitValue.CreatePercentArray(new float[] { 20f, 80f })).UseAllAvailableWidth();
    firsttable.SetPadding(0);
    firsttable.SetMargin(0);
    firsttable.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);
    //ImageData barCodeImageData = ImageDataFactory.Create(BarCodeWriterLablel(data.Tracking_No, 210, 20, true));
    //Image barCodeImage = new Image(barCodeImageData);

    #region first table
    firsttable.AddCell(new Cell().Add(new Paragraph("Image")).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingLeft(2f).SetBorder(Border.NO_BORDER));

    firsttable.AddCell(new Cell().Add(new Paragraph("Barcode")).SetTextAlignment(TextAlignment.CENTER).SetPaddingLeft(5f).SetPaddingTop(10f).SetPaddingBottom(5f).SetPaddingRight(10f).SetBorder(Border.NO_BORDER));
    firsttable.IsComplete();

    firsttable.AddCell(new Cell().Add(new Paragraph("AWB")).SetTextAlignment(TextAlignment.CENTER).SetPaddingTop(6f).SetBorder(Border.NO_BORDER));
    firsttable.AddCell(new Cell().Add(new Paragraph("4300015915")).SetTextAlignment(TextAlignment.CENTER).SetPaddingTop(3f).AddStyle(new Style().SetFontSize(16f).SetBold()).SetBorder(Border.NO_BORDER));
    firsttable.IsComplete();

    firsttable.AddCell(new Cell().Add(new Paragraph("Ref #")).SetTextAlignment(TextAlignment.CENTER).AddStyle(styleTopBorder));
    firsttable.AddCell(new Cell().Add(new Paragraph("Barcode")).SetTextAlignment(TextAlignment.RIGHT).AddStyle(styleTopBorder));
    firsttable.IsComplete();
    #endregion

    #region second table
    Table secondtable = new Table(new float[] { 5f, 95f }).SetWidth(UnitValue.CreatePercentValue(100));
    Paragraph p = new Paragraph("Shipper Details").AddStyle(new Style().SetFontSize(9f));
    p.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.NOWRAP);
    secondtable.AddCell(new Cell()
        .Add(p.SetRotationAngle(Math.PI / 2).SetVerticalAlignment(VerticalAlignment.MIDDLE)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE));

    Table secinnertable = new Table(new float[] { 25f, 25f, 25f, 25f }).SetWidth(UnitValue.CreatePercentValue(100));
    secinnertable.SetProperty(Property.BORDER_COLLAPSE, BorderCollapsePropertyValue.COLLAPSE);
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("Origin").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetBorder(Border.NO_BORDER)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("Destination").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("Product").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("Services").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("ShiperCityCode").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("RecipientCityCode").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("DOM / CDS").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("ServiceTypeName").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));

    secinnertable.AddCell(new Cell()
    .Add(new Paragraph("Weight").AddStyle(arial7))
    .AddStyle(styleTopRightBorder)
    .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell(1, 2)
        .Add(new Paragraph("Description of goods").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("Payment ACC").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.IsComplete();

    secinnertable.AddCell(new Cell()
        .Add(new Paragraph(" \n (kg)").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell(1, 3)
        .Add(new Paragraph("Description").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    secinnertable.IsComplete();
    secinnertable.AddCell(new Cell(1, 2)
        .Add(new Paragraph("GoodsOrigin").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("COD Value").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("PCs").AddStyle(arial7))
        .AddStyle(styleTopRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.IsComplete();
    secinnertable.AddCell(new Cell(1, 2)
        .Add(new Paragraph("ShiperCityCode").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("CostOfGoods" + "/-").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.AddCell(new Cell()
        .Add(new Paragraph("No_Of_Pieces").AddStyle(arial9Bold))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    secinnertable.IsComplete();

    secondtable.AddCell(new Cell().Add(secinnertable).SetBorder(Border.NO_BORDER));
    secinnertable.IsComplete();
    #endregion

    //#region third
    Table thirdtoptable = new Table(new float[] { 5f, 35f, 30f, 30f }).SetWidth(UnitValue.CreatePercentValue(100));
    thirdtoptable.AddCell(new Cell()
        .Add(new Paragraph(" ").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetRotationAngle(Math.PI / 2)
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));

    thirdtoptable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Account # " + "ShipperCode").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdtoptable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("PickupDate").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    thirdtoptable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Shipment_Date").AddStyle(arial7))
        .AddStyle(styleRightBorder)
        .SetTextAlignment(TextAlignment.CENTER));
    thirdtoptable.IsComplete();

    Table thirdtable = new Table(new float[] { 4f, 94f });
    thirdtable.SetPadding(0);
    thirdtable.SetBorder(Border.NO_BORDER);

    thirdtable.AddCell(new Cell()
        .Add(new Paragraph("Shipper Details").AddStyle(new Style().SetFontSize(9f)).SetRotationAngle(Math.PI / 2).SetVerticalAlignment(VerticalAlignment.MIDDLE)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetWidth(100f));

    Table thirdCentertable = new Table(new float[] { 18f, 82f });
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Name").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("SenderName").AddStyle(arial7Bold))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.IsComplete();
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Address").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("SenderStreet").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.IsComplete();

    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("City").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("SenderCity").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.IsComplete();

    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Tel").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("SenderMobile").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.IsComplete();

    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("Ref2").AddStyle(arial7))
        .AddStyle(styleNoBorder));
    thirdCentertable.AddCell(new Cell(1, 1)
        .Add(new Paragraph("").AddStyle(arial7))
        .AddStyle(styleNoBorder)
        .SetTextAlignment(TextAlignment.LEFT));
    thirdCentertable.IsComplete();

    thirdtable.AddCell(thirdCentertable).IsComplete();

   // thirdtable.AddCell(new Cell(1, 1)
   //.Add(new Paragraph("Consignce Details").AddStyle(arial7))
   //.SetHorizontalAlignment(HorizontalAlignment.CENTER)
   //.SetRotationAngle(Math.PI / 2)
   //.SetBackgroundColor(ColorConstants.LIGHT_GRAY));

   // Table fourCentertable = new Table(new float[] { 18f, 82f }).SetWidth(UnitValue.CreatePercentValue(100));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Name").AddStyle(arial7))
   //   .SetBorder(Border.NO_BORDER)
   //   .SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(5f).SetPaddingBottom(5f));

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Recipient_Name").AddStyle(arial7Bold)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(5f).SetPaddingBottom(5f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Attn Of").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(5f).SetPaddingBottom(5f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(5f).SetPaddingBottom(5f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Address").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(5f).SetPaddingBottom(5f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("LocationTo").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetHeight(48f).SetPaddingTop(5f).SetPaddingBottom(5f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Area").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("AreaName").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("City").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("RecipientCity").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Tel1").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("RecipientMobile").AddStyle(arial7)).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.IsComplete();

   // fourCentertable.AddCell(new Cell().Add(new Paragraph("Tel2").AddStyle(arial7)).SetBorderBottom(solidBorder).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));
   // fourCentertable.AddCell(new Cell().Add(new Paragraph("").AddStyle(arial7)).SetBorderBottom(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.LEFT).SetPaddingTop(2f).SetPaddingBottom(2f));

   // thirdtable.AddCell(fourCentertable).IsComplete();

    Table thirdoutertable = new Table(new float[] { 90f, 10f });
    thirdoutertable.SetHorizontalAlignment(HorizontalAlignment.LEFT);
    thirdoutertable.SetPadding(0);
    thirdoutertable.AddStyle(styleNoBorder);

    thirdoutertable.AddCell(thirdtable.SetWidth(100f));
    //Image barCodeImage = new Image(ImageDataFactory.Create(BarCodeWriterLablel(data.Tracking_No, 200, 20, true))).SetRotationAngle(Math.PI / 2);
    thirdoutertable.AddCell(new Cell()
        .Add(new Paragraph("Barcode").AddStyle(new Style().SetFontSize(9f)).SetRotationAngle(Math.PI / 2).SetVerticalAlignment(VerticalAlignment.MIDDLE)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE));
    thirdoutertable.IsComplete();

    //// #endregion
    //#endregion
    //#region last
    //Table fourtable = new Table(new float[] { 20f, 60f, 20f }).SetWidth(UnitValue.CreatePercentValue(100f));
    //fourtable.SetHorizontalAlignment(HorizontalAlignment.LEFT);
    //fourtable.SetPadding(0);

    //fourtable.AddCell(new Cell().Add(new Paragraph("Remarks").AddStyle(arial7).SetBorderLeft(Border.NO_BORDER).SetPaddingLeft(10f).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph("Instruction").AddStyle(arial7).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph("").AddStyle(arial7).SetBorderRight(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.IsComplete();

    //fourtable.AddCell(new Cell().Add(new Paragraph("Reference").AddStyle(arial7).SetBorderLeft(Border.NO_BORDER).SetPaddingLeft(10f).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph("Barcode").AddStyle(arial7).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph("0").AddStyle(arial7).SetBorderRight(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.IsComplete();

    //fourtable.AddCell(new Cell().Add(new Paragraph("Printed").AddStyle(arial7).SetBorderBottom(solidBorder).SetBorderLeft(Border.NO_BORDER).SetPaddingLeft(10f).SetPaddingBottom(3f).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).AddStyle(arial7).SetBorderBottom(solidBorder).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.AddCell(new Cell().Add(new Paragraph("").AddStyle(arial7).SetBorderBottom(solidBorder).SetBorderRight(Border.NO_BORDER).SetPaddingBottom(3f).SetTextAlignment(TextAlignment.LEFT)));
    //fourtable.IsComplete();
    //#endregion

    innnertable.AddCell(firsttable).SetPadding(0).SetMargin(0).AddStyle(styleNoBorder);
    innnertable.IsComplete();
    innnertable.AddCell(secondtable).SetPadding(0).SetMargin(0);
    innnertable.IsComplete();
    innnertable.AddCell(thirdtoptable).SetPadding(0).SetMargin(0).IsComplete();
    innnertable.AddCell(thirdoutertable).IsComplete();
    //innnertable.AddCell(fourtable).SetPadding(0).SetMargin(0).IsComplete();

    Table outerInnerTable = new Table(UnitValue.CreatePercentArray(new float[] { 30f, 70f })).UseAllAvailableWidth();
    outerInnerTable.AddCell(new Cell().Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER));
    outerInnerTable.AddCell(new Cell().Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
    outerInnerTable.IsComplete();
    //outerInnerTable.AddCell(new Cell().Add(new Paragraph("AWB       *" + "Tracking_No" + "*")).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
    outerInnerTable.IsComplete();

    outertable.AddCell(innnertable);
    outertable.AddCell(new Cell().Add(outerInnerTable).SetBorder(Border.NO_BORDER).SetRotationAngle(Math.PI / 2).SetWidth(100f));
    outertable.IsComplete();

    //Paragraph websiteParagraph = new Paragraph("www.shipra.com");
    //websiteParagraph.AddStyle(arial7);
    //Cell websiteCell = new Cell(2, 1).Add(websiteParagraph).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.CENTER);
    //outertable.AddCell(websiteCell);
    //outertable.IsComplete();
    document.Add(outertable);

  }
  public class BorderlessCell : Cell
  {
    public BorderlessCell(int rowSpan, int colSpan) : base(rowSpan, colSpan) { }
    public BorderlessCell() : base() { }
    public override T1 GetDefaultProperty<T1>(int property)
    {
      switch (property)
      {
        case Property.BORDER:
          return (T1)(Object)(Border.NO_BORDER);
        case Property.PADDING_BOTTOM:
        case Property.PADDING_LEFT:
        case Property.PADDING_RIGHT:
        case Property.PADDING_TOP:
          return (T1)(Object)(0);
        default:
          return base.GetDefaultProperty<T1>(property);
      }
    }
  }
}
