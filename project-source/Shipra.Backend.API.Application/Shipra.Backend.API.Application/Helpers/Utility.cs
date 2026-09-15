using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NPOI.Util;
using PhoneNumbers;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.Common;
using Shipra.Backend.API.Core.AppConfigAggregate;
using Shipra.Backend.API.Core.Models;
using static QRCoder.PayloadGenerator;

namespace Shipra.Backend.API.Application.Helpers;
public static class Utility
{
  public static PasswordValidationResult ValidatePassword(string? password)
  {
    var result = new PasswordValidationResult();

    if (string.IsNullOrWhiteSpace(password))
    {
      result.Errors.Add("Password is required.");
      result.IsValid = false;
      return result;
    }

    if (password.Length < 8)
      result.Errors.Add("Your password length must be at least 8.");

    if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
      result.Errors.Add("Your password must contain at least one uppercase letter.");

    if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
      result.Errors.Add("Your password must contain at least one lowercase letter.");

    if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]"))
      result.Errors.Add("Your password must contain at least one number.");

    if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[^A-Za-z0-9]"))
      result.Errors.Add("Your password must contain at least one special character.");

    result.IsValid = !result.Errors.Any();
    return result;
  }

  public static bool ValidateLatitude(string latitude)
  {
    string latitudePattern = @"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)$";
    return Regex.IsMatch(latitude, latitudePattern);

  }
  public static bool ValidateLongitude(string longitude)
  {
    string longitudePattern = @"^[-+]?((([1-9]?\d|1[0-7]\d)(\.\d+)?)|180(\.0+)?)$";
    return Regex.IsMatch(longitude, longitudePattern);
  }

  public static DateTimeOffset ConvertUTCDateToDateTimeOffSet(DateTime requestDate)
  {
    TimeZoneInfo currentTimeZone = TimeZoneInfo.Local;
    TimeZoneInfo desiredTimeZone = TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone.Id); // Replace with your desired time zone
    return TimeZoneInfo.ConvertTimeFromUtc(requestDate, desiredTimeZone);
  }

  public static bool IsValidEmail(string email)
  {
    // Define the regular expression pattern for a valid email
    string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    // Create a Regex object
    Regex regex = new Regex(pattern);

    // Use the IsMatch method to check if the email matches the pattern
    return regex.IsMatch(email);
  }
  public static string ValidateUrlWithHttps(string url)
  {
    // Define a regular expression pattern for matching URLs
    Regex urlPattern = new Regex(@"^https?://(?:www\.)?(.+)$");

    // Check if the URL matches the pattern
    Match match = urlPattern.Match(url);

    if (match.Success)
    {
      // If the URL already starts with http:// or https://, return the original URL
      return url;
    }
    else
    {
      // If not, add https:// to the beginning of the URL and return the modified URL
      return "https://" + url;
    }
  }

  public static bool IsValidUrlWithoutHttp(string url)
  {
    // Define the regular expression pattern for a valid URL without http/https
    string pattern = @"^(www\.)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    // Create a Regex object
    Regex regex = new Regex(pattern);

    // Use the IsMatch method to check if the URL matches the pattern
    return regex.IsMatch(url);
  }

  public static bool IsValidShopifyUrl(string shopUrl)
  {
    // Define the regular expression pattern for a valid Shopify shop URL without http/https
    string pattern = @"^[a-zA-Z0-9-]+\.myshopify\.com$";

    // Create a Regex object
    Regex regex = new Regex(pattern);

    // Use the IsMatch method to check if the Shopify shop URL matches the pattern
    return regex.IsMatch(shopUrl);
  }
  public static bool IsValidWooCommerceUrl(string shopUrl)
  {
    //string pattern = @"^https:\/\/[a-zA-Z0-9-]+\.com$";

    // Define the regular expression pattern for a valid WooCommerce shop URL with https
    // The pattern now supports various TLDs, including those with multiple parts (e.g., .co.uk)
    string pattern = @"^https:\/\/[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(\.[a-zA-Z]{2,})?$";
    // Create a Regex object
    Regex regex = new Regex(pattern);

    // Use the IsMatch method to check if the WooCommerce shop URL matches the pattern
    return regex.IsMatch(shopUrl);
  }
  public static bool IsValidWixUrl(string url)
  {
    if (string.IsNullOrWhiteSpace(url))
      return false;

    url = url.Trim(); // Remove leading/trailing spaces
    // Match Wix site URLs: https://yourname.wixsite.com
    string wixPattern = @"^https:\/\/[a-zA-Z0-9\-]+\.wixsite\.com(\/.*)?$";

    // Match custom domains with optional www, subdomains, path, or query string
    string customDomainPattern = @"^https:\/\/(www\.)?[a-zA-Z0-9\-]+(\.[a-zA-Z]{2,})+(\/.*)?$";
    return Regex.IsMatch(url, wixPattern) || Regex.IsMatch(url, customDomainPattern);
  }
  public static string ConvertStringListToCsv<T>(List<T> inputList)
  {
    var csvString = string.Empty;

    if (inputList == null || inputList.Count == 0)
    {
      return csvString;
    }
    csvString = string.Join(",", inputList);

    return csvString;
  }
  public static string RemoveValuesFromString(string str, string valuesToRemove)
  {
    // Split the strings into arrays of integers
    var strArray = str.Split(',').Select(int.Parse).ToList();
    var valuesToRemoveArray = valuesToRemove.Split(',').Select(int.Parse).ToList();

    // Remove values from strArray that are present in valuesToRemoveArray
    strArray.RemoveAll(value => valuesToRemoveArray.Contains(value));

    // Join the remaining values into a string
    string result = string.Join(",", strArray);

    return result;
  }
  public static bool ContainsWord(List<string> errorMessages, string? word)
  {
    foreach (var errorMessage in errorMessages)
    {
      if (errorMessage.Contains(word!))
      {
        return true;
      }
    }
    return false;
  }
  public static string? GetFirstRequiredFieldKey(string json)
  {
    // Parse JSON
    if (!string.IsNullOrEmpty(json))
    {
      JObject jsonObject = JObject.Parse(json);
      JArray keys = (JArray)jsonObject["keys"]!;

      foreach (var key in keys)
      {
        string keyName = key.ToString();
        if (jsonObject[keyName]?["required"]?.ToObject<bool>() == true)
        {
          return keyName; // Return the key of the first required field
        }
      }
    }

    return null; // Return null if no required field is found
  }
  public static string GetPublicKey()
  {
    //RSACryptoServiceProvider rsaPublic = new RSACryptoServiceProvider();
    // Export the public key as XML string
    return Guid.NewGuid().ToString();
  }
  public static string GetRandomKey()
  {
    //RSACryptoServiceProvider rsaPublic = new RSACryptoServiceProvider();
    // Export the public key as XML string
    return Guid.NewGuid().ToString();
  }

  public static string GetSecretKey()
  {
    //RSACryptoServiceProvider rsaSecret = new RSACryptoServiceProvider();
    // Export the private key as XML string
    return Guid.NewGuid().ToString();
  }

  #region encryption
  public static string Encrypt(string input, string? encryptKey)
  {
    if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(encryptKey))
    {
      byte[] inputArray = Encoding.UTF8.GetBytes(input);

      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Encoding.UTF8.GetBytes(encryptKey);
        aesAlg.Mode = CipherMode.CBC; // You may choose a different mode based on your requirements
        aesAlg.Padding = PaddingMode.PKCS7;

        // Generate a random IV (Initialization Vector)
        aesAlg.IV = new byte[aesAlg.BlockSize / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
          rng.GetBytes(aesAlg.IV);
        }

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            csEncrypt.Write(inputArray, 0, inputArray.Length);
            csEncrypt.FlushFinalBlock();
          }

          byte[] encryptedBytes = msEncrypt.ToArray();
          byte[] resultArray = new byte[aesAlg.IV.Length + encryptedBytes.Length];
          Buffer.BlockCopy(aesAlg.IV, 0, resultArray, 0, aesAlg.IV.Length);
          Buffer.BlockCopy(encryptedBytes, 0, resultArray, aesAlg.IV.Length, encryptedBytes.Length);

          return Convert.ToBase64String(resultArray);
        }
      }
    }

    return string.Empty;
  }
  public static string Decrypt(string input, string? encryptKey)
  {
    if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(encryptKey))
    {
      byte[] inputArray = Convert.FromBase64String(input);

      using (Aes aesAlg = Aes.Create())
      {
        aesAlg.Key = Encoding.UTF8.GetBytes(encryptKey);
        aesAlg.Mode = CipherMode.CBC; // You may choose a different mode based on your requirements
        aesAlg.Padding = PaddingMode.PKCS7;

        // Extract IV from the inputArray
        byte[] iv = new byte[aesAlg.BlockSize / 8];
        Buffer.BlockCopy(inputArray, 0, iv, 0, iv.Length);

        // Set IV for decryption
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msDecrypt = new MemoryStream(inputArray, iv.Length, inputArray.Length - iv.Length))
        {
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
              return srDecrypt.ReadToEnd();
            }
          }
        }
      }
    }

    return string.Empty;
  }
  #endregion
  #region validate phone number
  public static ValidatePhoneResponseModel ValidateMobileAndGetNumber(string? number, string? countryCode = "AE", bool? checkValid = true)
  {
    ValidatePhoneResponseModel responseModel = new ValidatePhoneResponseModel();
    try
    {
      if (string.IsNullOrWhiteSpace(number))
      {
        responseModel.IsValid = false;
        return responseModel;
      }

      var phoneNumberUtil = PhoneNumberUtil.GetInstance();

      // Determine if we should pass countryCode or null
      string? regionCode = number.StartsWith("+") ? null : countryCode;

      var parsedPhoneNumber = phoneNumberUtil.Parse(number, regionCode);
      var formattedPhoneNumber = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.INTERNATIONAL);
      Console.WriteLine($"Parsed Number: {parsedPhoneNumber.CountryCode} {parsedPhoneNumber.NationalNumber}");
      Console.WriteLine($"Formatted Number: {formattedPhoneNumber}");
      responseModel.IsValid = checkValid.GetValueOrDefault()
          ? phoneNumberUtil.IsValidNumber(parsedPhoneNumber)
          : true;

      responseModel.PhoneNumber = formattedPhoneNumber;
    }
    catch (NumberParseException)
    {
      responseModel.IsValid = false;
    }

    return responseModel;
  }

  //public static ValidatePhoneResponseModel ValidateMobileAndGetNumber(string? number, string? countryCode = "AE", bool? checkValid = true)
  //{
  //  ValidatePhoneResponseModel responseModel = new ValidatePhoneResponseModel();
  //  try
  //  {
  //    var phoneNumberUtil = PhoneNumberUtil.GetInstance();
  //    //var phoneNumber = phoneNumberUtil.Parse(number, null);
  //    var parsedPhoneNumber = phoneNumberUtil.Parse(number, countryCode);
  //    var formattedPhoneNumber = phoneNumberUtil.Format(parsedPhoneNumber, PhoneNumberFormat.INTERNATIONAL);
  //    if (checkValid.GetValueOrDefault())
  //    {
  //      var isValid = phoneNumberUtil.IsValidNumber(parsedPhoneNumber);
  //      responseModel.IsValid = isValid;
  //    }
  //    else
  //    {
  //      responseModel.IsValid = true;
  //    }
  //    responseModel.PhoneNumber = formattedPhoneNumber;
  //  }
  //  catch (Exception)
  //  {
  //    responseModel.IsValid = false;
  //  }

  //  return responseModel;
  //}
  #endregion
  #region json

  public static string RemoveWhitespace(string input)
  {
    if (string.IsNullOrEmpty(input))
    {
      return input;
    }
    return Regex.Replace(input, @"\s+", "");
  }
  #endregion
  public static bool IsValidJson(string jsonString)
  {
    try
    {
      JToken.Parse(jsonString);  // Tries to parse the JSON
      return true;
    }
    catch (JsonReaderException)
    {
      return false;  // Invalid JSON
    }
  }
  public enum DateTimeConversionType
  {
    Utc,
    Local,
    None // Ignore offset
  }

  public static DateTime ConvertDateTimeOffsetToDateTime(DateTimeOffset dateTimeOffset, DateTimeConversionType conversionType)
  {
    switch (conversionType)
    {
      case DateTimeConversionType.Utc:
        return dateTimeOffset.UtcDateTime;
      case DateTimeConversionType.Local:
        return dateTimeOffset.LocalDateTime;
      case DateTimeConversionType.None:
        return dateTimeOffset.DateTime; // Ignores the offset, returns the same local time
      default:
        throw new ArgumentException("Invalid conversion type.");
    }
  }


  private static readonly Random Random = new Random();
  public static string GenerateRandomPassword(int minLength = 8, int maxLength = 12)
  {
    if (minLength < 8 || maxLength > 12 || minLength > maxLength)
    {
      throw new ArgumentException("Password length must be between 8 and 12 characters.");
    }

    const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
    const string numbers = "0123456789";
    const string specialChars = "@#$%^&*!";
    string allChars = upperCase + lowerCase + numbers + specialChars;

    // Ensure the password contains at least one of each required character type
    var password = new StringBuilder();
    password.Append(upperCase[Random.Next(upperCase.Length)]);
    password.Append(lowerCase[Random.Next(lowerCase.Length)]);
    password.Append(numbers[Random.Next(numbers.Length)]);
    password.Append(specialChars[Random.Next(specialChars.Length)]);

    // Fill the remaining characters up to the randomly chosen length
    int totalLength = Random.Next(minLength, maxLength + 1);
    for (int i = password.Length; i < totalLength; i++)
    {
      password.Append(allChars[Random.Next(allChars.Length)]);
    }

    // Shuffle the characters to randomize the order
    return new string(password.ToString().ToCharArray().OrderBy(_ => Random.Next()).ToArray());
  }

  //use inside controller to get all methods
  //public static dynamic GetActionMethodNames()
  //{
  //  Assembly assembly = Assembly.GetExecutingAssembly();
  //  IEnumerable<Type> types = assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).OrderBy(x => x.Name);
  //  List<dynamic> controllerActions = new List<dynamic>();
  //  foreach (Type cls in types)
  //  {
  //    dynamic controller = new System.Dynamic.ExpandoObject();
  //    controller.Controller = cls.Name.Replace("Controller", "");
  //    controller.Actions = new List<string>();
  //    IEnumerable<MethodInfo> memberInfo = cls.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).OrderBy(x => x.Name);
  //    foreach (MethodInfo method in memberInfo)
  //    {
  //      if (method.DeclaringType!.IsPublic && !method.IsDefined(typeof(NonActionAttribute)))
  //      {
  //        controller.Actions.Add(method.Name);
  //      }
  //    }
  //    controllerActions.Add(controller);
  //  }

  //  return controllerActions;
  //}
  // Helper method to trim the value to the desired precision
  public static decimal? TrimToPrecision(decimal? value, int totalDigits = 8, int decimalPlaces = 6) //8,6 for db saving
  {
    if (!value.HasValue)
      return null; // Return null if value is null

    decimal maxValue = (decimal)Math.Pow(10, totalDigits - decimalPlaces) - (decimal)Math.Pow(10, -decimalPlaces);
    decimal scale = (decimal)Math.Pow(10, decimalPlaces);

    decimal trimmedValue = Math.Truncate(value.Value * scale) / scale;

    // Ensure the trimmed value doesn't exceed the allowed max digits
    return trimmedValue > maxValue ? maxValue : trimmedValue;
  }

}
