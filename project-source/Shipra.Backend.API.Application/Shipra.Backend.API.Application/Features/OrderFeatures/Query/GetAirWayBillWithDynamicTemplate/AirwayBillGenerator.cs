using System.Text;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
public class AirwayBillGenerator
{
  private readonly IBarcodeGenerate _barcodeGenerate;

  public AirwayBillGenerator(IBarcodeGenerate barcodeGenerate)
  {
    _barcodeGenerate = barcodeGenerate;
  }
  public StringBuilder GetAwbModifiedHtml(dynamic data, string outputFolder, string templatePath, DirectoryHelper directoryHelper)
  {
    int count = 0;
    StringBuilder sb = new StringBuilder();

    #region barcode images
    #region shipper ref abrcode
    string carrierBarCodeBase64 = string.Empty;

    #endregion
    string shipperrefBarCodeBase64 = string.Empty;
    if (!string.IsNullOrEmpty(data.RefNo))
    {
      shipperrefBarCodeBase64 = _barcodeGenerate.CreateBase64(data.RefNo);
    }
    ////barcode for report
    string orderBarCodeBase64 = _barcodeGenerate.CreateBase64(data.OrderNo);
    if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
    {
      carrierBarCodeBase64 = _barcodeGenerate.CreateBase64(data.CarrierTrackingNo);
    }
    #endregion
    Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);
 
    string receiverTitle = "Receiver";
    string senderTitle = "Store";
    if (data.OrderDeliveryTypeId == (int)EnumOrderDeliveryType.Reverse)
    {
      // Swap sender and receiver for reverse orders
      receiverTitle = "Sender";
      senderTitle = "Receiver";
    } 
    //// 
    if (data.OrderItems.Count == 0)
    {

      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, ""); 

      #region company logo
      var logoPath = ApplicationConstants.ShipraLogo;
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
      #endregion

      if (directoryHelper.CheckDirectoryExistAndCreate(outputFolder))
      {
        #region Order no  
        if (!string.IsNullOrEmpty(orderBarCodeBase64))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode}}", $"data:image/png;base64,{orderBarCodeBase64}");
        }
        #endregion
        #region optional barcode

        #region carrier tracking no
        if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl br talc "" style=""text-align:left"">
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Carrier Tracking No</div>
                                    <div class=""text-below text-below-text"">{data.CarrierTrackingNo}</div>
                                </td>");


          #region 4d1.html
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcodeD2a5}}", $@" <td class=""w-100"">
                            <table class=""w-100"">
                                <tr>
                                    <td colspan=""12"" class=""bottom-carrier-barcode"">
                                        <img class=""p-2 barcode-image bottom-carrier-barcode-td-img"" src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                        <p class=""textSmall1"">Tracking No: {data.CarrierTrackingNo}</p>
                                    </td>
                                </tr>
                            </table>
                        </td> ");
          #endregion
        }
        #endregion
        #region shipper ref number
        if (!string.IsNullOrEmpty(data.RefNo))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb bl br talc "" >
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Shipper Ref</div>
                                    <div class=""text-below text-below-text"">{data.RefNo}</div>
                                </td>");

          #region 4d1.html
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode4d1}}", $@" <td>
                <img class=""p-2 barcode-image"" src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                <p class=""textSmall1"">Shipper Ref: {data.RefNo}</p>
            </td>");
          #endregion
          //modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperBarcode}}", $"data:image/png;base64,{shipperrefBarCodeBase64}");
        }
        #endregion
        #endregion

      }
      #region 4d1.html
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode4d1}}", $@" <td>
                <img class=""p-2 barcode-image"" src=""data:image/png;base64,{orderBarCodeBase64}"" />
                <p class=""textSmall1""Order No: {data.OrderNo}</p>
            </td>");
      #endregion
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{itemCount}}", count + " / " + data.ItemsCount);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());
      #region MyRegion 
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{senderTitle}}", senderTitle);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{receiverTitle}}", receiverTitle); 
      #endregion
      sb.Append(modifiedHtmlContent);
      sb.Append("<div style=\"page-break-after: always;\"></div>");
    }
    else
    {
      foreach (var item in data.OrderItems)
      {
        count++;
        string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, ""); 
        #region company logo
        var logoPath = ApplicationConstants.ShipraLogo;
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
        #endregion

        if (directoryHelper.CheckDirectoryExistAndCreate(outputFolder))
        {
          #region Order Barcode  
          if (!string.IsNullOrEmpty(orderBarCodeBase64))
          {
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode}}", $"data:image/png;base64,{orderBarCodeBase64}");
          }
          #endregion


          #region optional barcode

          #region carrier tracking no
          if (!string.IsNullOrEmpty(data.CarrierTrackingNo))
          {
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl br talc "" style=""text-align:left"">
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Carrier Tracking No</div>
                                    <div class=""text-below text-below-text"">{data.CarrierTrackingNo}</div>
                                </td>");


            #region 4d1.html
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcodeD2a5}}", $@" <td class=""w-100"">
                            <table class=""w-100"">
                                <tr>
                                    <td colspan=""12"" class=""bottom-carrier-barcode"">
                                        <img class=""p-2 barcode-image bottom-carrier-barcode-td-img"" src=""data:image/png;base64,{carrierBarCodeBase64}"" />
                                        <p class=""textSmall1"">Tracking No: {data.CarrierTrackingNo}</p>
                                    </td>
                                </tr>
                            </table>
                        </td> ");
            #endregion
          }
          #endregion
          #region shipper ref number
          if (!string.IsNullOrEmpty(data.RefNo))
          {
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb bl br talc "" >
                                    <img class=""p-2""
                                         height=""50"" 
                                         src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                                    <div class=""text-below text-below-text-heading"">Shipper Ref</div>
                                    <div class=""text-below text-below-text"">{data.RefNo}</div>
                                </td>");

            #region 4d1.html
            modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode4d1}}", $@" <td class='w-40'>
                <img class=""p-2 barcode-image"" src=""data:image/png;base64,{shipperrefBarCodeBase64}"" />
                <p class=""textSmall1"">Shipper Ref: {data.RefNo}</p>
            </td>");
            #endregion

          }
          #endregion
          #endregion
        }
        if (string.IsNullOrEmpty(carrierBarCodeBase64) && string.IsNullOrEmpty(shipperrefBarCodeBase64))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", $@"<td class=""bt bb bl talc "" ></td>");
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", $@"<td class=""bt bb br talc "" ></td>");
        }
        if (string.IsNullOrEmpty(carrierBarCodeBase64))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcode}}", "");
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierBarcodeD2a5}}", "");
        }
        if (string.IsNullOrEmpty(shipperrefBarCodeBase64))
        {
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode}}", "");
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipperRefBarcode4d1}}", "");
        }
        #region 4d1.html
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode4d1}}", $@" <td class='w-50'>
                <img class=""p-2 barcode-image"" src=""data:image/png;base64,{orderBarCodeBase64}"" />
                <p class=""textSmall1"">Order No: {data.OrderNo}</p>
            </td>");
        #endregion
        //#region 4d1.html
        //modifiedHtmlContent = modifiedHtmlContent.Replace("{{orderNoBarcode4d1}}", $@" <td>
        //        <img class=""p-2 barcode-image"" src=""data:image/png;base64,{orderBarCodeBase64}"" />
        //        <p class=""textSmall1""Order No: {data.OrderNo}</p>
        //    </td>");
        //#endregion
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{itemCount}}", count + " / " + data.ItemsCount);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());
        #region MyRegion 
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{senderTitle}}", senderTitle);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{receiverTitle}}", receiverTitle);
        #endregion
        sb.Append(modifiedHtmlContent);
        sb.Append("<div style=\"page-break-after: always;\"></div>");
      }
    }
    return sb;
  }

  private string GetRecieverInfo(Dictionary<string, object> replacements,int orderDeliveryTypeId)
  {
    string title = "Receiver";
    if (orderDeliveryTypeId == (int)EnumOrderDeliveryType.Reverse)
    {
      // Swap sender and receiver for reverse orders
      title = "Sender";
    }
    string htmlTemplate = @$"
<tr>
    <td class='w-100'>
        <table class='w-100'>
            <tr>
                <td class='bt bl talc w-5' rowspan='7' style='background-color:lightgray;'>
                    <span style='display: block; transform-origin: top right; transform: rotate(90deg) translate(-100%); margin-top:0%; white-space: nowrap;width: 23px;font-weight: 700;'>
                        {title}
                    </span>
                </td>
                <td class='bt bl w-10'>Name</td>
                <td class='bt br w-50'>{{customerName}}</td>
            </tr>
            <tr>
                <td class='bl w-20'>Address</td>
                <td class='br w-80'>{{customerAddress}}</td>
            </tr>
            <tr>
                <td class='bl w-20'>Country</td>
                <td class='br w-80'>{{consigneCountryName}}</td>
            </tr>
            <tr>
                <td class='bl w-20'>Mobile 1</td>
                <td class='br w-80'>{{mobile1}}</td>
            </tr>
            <tr>
                <td class='bl w-20'>Mobile 2</td>
                <td class='br w-80'>{{mobile2}}</td>
            </tr>
        </table>
    </td>
</tr>";

    foreach (var keyValue in replacements)
    {
      string placeholder = "{" + keyValue.Key + "}";
      if (keyValue.Value != null)
      {
        htmlTemplate = htmlTemplate.Replace(placeholder, keyValue.Value.ToString());
      }
    }

    return htmlTemplate;
  }

  private string GetSenderInfo(Dictionary<string, object> replacements, int orderDeliveryTypeId)
  {
    string title = "Store";
    if (orderDeliveryTypeId == (int)EnumOrderDeliveryType.Reverse)
    {
      // Swap sender and receiver for reverse orders
      title = "Receiver";
    } 
    string htmlTemplate = @$" <tr>
                        <td class=""w-100"">
                            <table class=""w-100"">
                                <tr>
                                    <td class=""bt bl talc w-5""
                                        rowspan=""6"" style=""background-color:lightgray;"">
                                        <span style=""display: block;
                                                        transform-origin: top right; transform: rotate(90deg) translate(-100%);/* transform: rotate(90deg) translate(0, -100%); */margin-top:0%; white-space: nowrap;width: 23px;font-weight: 700;"">
                                            {title}
                                        </span>
                                    </td>
                                    <td class=""bt bl w-20"">Store Name</td>
                                    <td class=""bt br w-80"">{{storeName}}</td>
                                </tr>
                                <tr>
                                    <td class=""bl w-20"">Address</td>
                                    <td class=""br w-80"">
                                        {{storeAddress}}
                                    </td>
                                </tr>
                                <tr>
                                    <td class=""bl w-20"">Country</td>
                                    <td class=""br w-80"">{{storeCountry}}</td>
                                </tr>
                                <!--<tr>
                    <td class=""bl w-20"">Region</td>
                    <td class=""br w-80"">{{storeRegion}}</td>
                </tr>
                <tr>
                    <td class=""bl w-20"">City</td>
                    <td class=""br w-80"">{{storeCity}}</td>
                </tr>-->
                                <tr>
                                    <td class=""bl w-20"">Tel</td>
                                    <td class=""br w-80"">{{customerServiceNo}}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>";
    foreach (var keyValue in replacements)
    {
      string placeholder = "{" + keyValue.Key + "}";
      if (keyValue.Value != null)
      { 
        htmlTemplate = htmlTemplate.Replace(placeholder, keyValue.Value.ToString());
      }
    }

    return htmlTemplate;
  }
}
