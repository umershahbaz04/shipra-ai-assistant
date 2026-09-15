using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetBarcode;
using Type = NetBarcode.Type;

namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface IBarcodeGenerate
{
  string CreateBase64(string code, int height = 100, int width = 500, Type type = Type.Code128, bool isShowLabel = false);
}
