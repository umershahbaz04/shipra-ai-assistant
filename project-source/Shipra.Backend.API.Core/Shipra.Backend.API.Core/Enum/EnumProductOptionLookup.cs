using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Enum;
public enum EnumProductOptionLookup
{
  Size = 1,
  Company = 2,
  Color = 3,
  Model = 4,
  Other = 5
}

public static class EnumProductOptionLookupHelper
{
  public static string GetEnumString(EnumProductOptionLookup value)
  {
    switch (value)
    {
      case EnumProductOptionLookup.Size:
        return "Size";
      case EnumProductOptionLookup.Company:
        return "Company";
      case EnumProductOptionLookup.Color:
        return "Color";
      case EnumProductOptionLookup.Model:
        return "Model";
      case EnumProductOptionLookup.Other:
        return "Other";
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }

  public static EnumProductOptionLookup GetEnumDefault(int value)
  {
    switch (value)
    {
      case 1:
        return EnumProductOptionLookup.Size;
      case 2:
        return EnumProductOptionLookup.Company;
      case 3:
        return EnumProductOptionLookup.Color;
      case 4:
        return EnumProductOptionLookup.Model;
      case 5:
        return EnumProductOptionLookup.Other;
      default:
        throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid enum value.");
    }
  }
}
