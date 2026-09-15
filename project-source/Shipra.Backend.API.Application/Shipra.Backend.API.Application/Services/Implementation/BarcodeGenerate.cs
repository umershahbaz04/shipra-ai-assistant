using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBarcode;
using Shipra.Backend.API.Application.Services.Interfaces;
using Type = NetBarcode.Type;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class BarcodeGenerate : IBarcodeGenerate
{
  public string CreateBase64(string code, int height = 100, int width = 500, Type type = Type.Code128, bool isShowLabel = false)
  {
    var barcode = new Barcode(code, type, isShowLabel, width, height);
    return barcode.GetBase64Image();
  }
}
