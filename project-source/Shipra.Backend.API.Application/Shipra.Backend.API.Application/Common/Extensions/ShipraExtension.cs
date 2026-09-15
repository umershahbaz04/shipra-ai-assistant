using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.Common.Extensions;

public static class ShipraExtension
{
  public static string FormatToPhoneNumber(this string phoneNumber)
  {
    if (!string.IsNullOrEmpty(phoneNumber))
    {
      var tempPhone = phoneNumber.Replace("-", string.Empty);
      if (tempPhone.Length >= 10)
      {
        var phone = new StringBuilder();

        phone.Append(tempPhone.Substring(0, 3)).Append("-");
        phone.Append(tempPhone.Substring(3, 3)).Append("-");
        phone.Append(tempPhone.Substring(6, 4));

        return phone.ToString();
      }
    }

    return phoneNumber;
  }
}
